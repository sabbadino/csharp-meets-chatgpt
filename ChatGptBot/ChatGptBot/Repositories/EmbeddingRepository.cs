﻿using ChatGptBot.Dtos.Embeddings;
using ChatGptBot.Ioc;
using ChatGptBot.Repositories.Entities;
using ChatGptBot.Services;
using ChatGptBot.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Repositories
{
    public interface IEmbeddingRepository
    {
        Task AddEmbedding(EmbeddingForDb embeddingForDb);
        Task<List<Embedding>> LoadSet(string code);

        Task UpsertEmbeddingSet(EmbeddingSet embeddingSet);
        Task<List<EmbeddingSet>> SearchEmbeddingSet();

        Task<EmbeddingSet> EmbeddingSetById(Guid id);
    }

    public class EmbeddingRepository : IEmbeddingRepository, ISingletonScope
    {
        private readonly string _cnString;

        public EmbeddingRepository(IOptions<Storage> storage)
        {
            _cnString = storage.Value.ConnectionString;
        }
        public async Task AddEmbedding(EmbeddingForDb embeddingForDb)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            await InsertEmbeddingHeader(embeddingForDb,cn);
            await InsertVectorItem(embeddingForDb,cn);
        }

        public async Task<List<Embedding>> LoadSet(string code)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand(@"select vectorValue,embeddingId, tokens, text from Embeddings e
                join EmbeddingVectors v on e.id=v.embeddingId 
                join EmbeddingsSet es on es.id=e.setId
                where es.code=@code 
                order by e.id,v.vectorOrder");

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "code",
                Value = code,
            });
            cmd.Connection = cn;
            await using var reader = await cmd.ExecuteReaderAsync();
            var ret = new List<Embedding>();
            var activeEmbedding = new Embedding();
            while (reader.Read())
            {
                var currentEmbeddingId = (Guid) reader["embeddingId"];
                if (currentEmbeddingId != activeEmbedding.Id)
                {
                    activeEmbedding = new Embedding
                    {
                        Id = currentEmbeddingId, Tokens = (int)reader["tokens"] 
                        , Text = (string) reader["text"] 
                    };
                    ret.Add(activeEmbedding);
                }
                activeEmbedding.VectorValues.Add((float)reader["vectorValue"]);
            }
            return ret;
        }

        public async Task<EmbeddingSet> GetEmbeddingSetById(Guid setId)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand(@"SELECT id,code,description FROM EmbeddingsSet where id=@id");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "id",
                Value = setId,
            });
            cmd.Connection = cn;
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                throw new Exception($"embedding set with id {setId} not found");
            }

            reader.Read();
                var ret = new EmbeddingSet
                {
                    Id = reader.GetGuid(0),
                    Code = reader.GetString(1),
                    Description = reader.GetString(2),
                };
            return ret;
        }

        public async Task UpsertEmbeddingSet(EmbeddingSet embeddingSet)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand(@"IF NOT EXISTS 
                (SELECT * FROM EmbeddingsSet  
                        WHERE id=@id)
                BEGIN 
                    INSERT INTO EmbeddingsSet (id, Code, description)
                    VALUES (@id, @code, @description)
                END");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "id",
                Value = embeddingSet.Id,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "code",
                Value = embeddingSet.Code,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "description",
                Value = embeddingSet.Description,
            });
            cmd.Connection = cn;
            await cmd.ExecuteNonQueryAsync();
                
        }

        public async Task <List<EmbeddingSet>> SearchEmbeddingSet()
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand(@"SELECT id,code,description FROM EmbeddingsSet");
            cmd.Connection = cn;
            await using var reader = await cmd.ExecuteReaderAsync();
            var ret = new List<EmbeddingSet>();
            while (reader.Read())
            {
                ret.Add(new EmbeddingSet
                {
                    Id = reader.GetGuid(0),
                    Code = reader.GetString(1),
                    Description = reader.GetString(2),
                });
            }
            return ret;
        }

        public async Task<EmbeddingSet> EmbeddingSetById(Guid id)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand(@"SELECT id,code,description FROM EmbeddingsSet");
            cmd.Connection = cn;
            await using var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SingleResult);
            if (!reader.HasRows)
            {
                throw new Exception($"embeddingSet with id {id} not found");
            }
            else
            {
                reader.Read();
                return new EmbeddingSet
                {
                    Id = reader.GetGuid(0),
                    Code = reader.GetString(1),
                    Description = reader.GetString(2),
                };
            }
        }

        private static async Task InsertVectorItem(EmbeddingForDb embeddingForDb, SqlConnection cn)
        {
            
            
            await Parallel.ForEachAsync(embeddingForDb.VectorValues, async (vectorValue, _) =>
            {
                var cmd = new SqlCommand(@"INSERT INTO EmbeddingVectors
                    ( embeddingId, vectorValue,vectorOrder)
                    VALUES (@embeddingId, @vectorValue,@vectorOrder)");
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "embeddingId",
                    Value = embeddingForDb.Id,
                });
                var valueParameter = new SqlParameter
                {
                    ParameterName = "vectorValue",
                };
                cmd.Parameters.Add(valueParameter);
                var vectorOrderParameter = new SqlParameter
                {
                    ParameterName = "vectorOrder",

                };
                cmd.Parameters.Add(vectorOrderParameter);
                cmd.Connection = cn;
                {
                    valueParameter.Value = vectorValue.Value;
                    vectorOrderParameter.Value = vectorValue.Index;
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        private static async Task InsertEmbeddingHeader(EmbeddingForDb embeddingForDb, SqlConnection cn)
        {
            var cmd = new SqlCommand(@"INSERT INTO Embeddings
                    (id, text, setId,tokens)
                    VALUES (@id, @text, @setId,@tokens)");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "id",
                Value = embeddingForDb.Id,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "text",
                Value = embeddingForDb.Text,
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "setId",
                Value = embeddingForDb.SetId,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "tokens",
                Value = embeddingForDb.Tokens,
            });

            cmd.Connection = cn;
            await cmd.ExecuteNonQueryAsync();
        }

        
    }


}
