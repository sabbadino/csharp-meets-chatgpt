using Azure.AI.OpenAI;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using myMscChatGpt.Ioc;
using myMscChatGpt.Repositories.Entities;
using myMscChatGpt.Services;
using myMscChatGpt.Settings;

namespace myMscChatGpt.Repositories
{
    public interface IConversationRepository
    {
        Task StoreConversationItem(ConversationItem conversationItem);
        Task <List<ConversationItem>> LoadConversation(Guid conversationId);
    }

    public class ConversationRepository : IConversationRepository, ISingletonScope
    {
        private readonly string _cnString;

        public ConversationRepository(IOptions<Storage> storage)
        {
            _cnString = storage.Value.ConnectionString;
        }

        public async Task StoreConversationItem(ConversationItem conversationItem)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            await CreateConversationHeaderIfRequired(cn, conversationItem);
            var cmd = new SqlCommand($"insert into conversationItems (id,conversationHeaderId,text,chatrole,at,tokens) values (@id,@conversationHeaderId, @text,@chatrole,@at, @tokens)");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "id",
                Value = conversationItem.Id,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationHeaderId",
                Value = conversationItem.ConversationId,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "text",
                Value = conversationItem.Text,
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "chatrole",
                Value = ChatRoleToString(conversationItem.ChatRole)
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "at",
                Value = conversationItem.At,
            }); cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "tokens",
                Value = conversationItem.Tokens,
            });

            cmd.Connection = cn;
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task CreateConversationHeaderIfRequired(SqlConnection cn, ConversationItem conversationItem)
        {
            var cmd = new SqlCommand(@"IF NOT EXISTS 
                (SELECT * FROM conversationHeader  
                        WHERE id=@conversationHeaderId)
                BEGIN 
                    INSERT INTO conversationHeader (id, text)
                    VALUES (@conversationHeaderId, @text)
                END");
            cmd.Connection = cn;
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationHeaderId",
                Value = conversationItem.ConversationId,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "text",
                Value = conversationItem.Text,
            });
            
            await cmd.ExecuteNonQueryAsync();
        }

        private string ChatRoleToString(ChatRole chatRole)
        {
            if (chatRole == ChatRole.Assistant) return "A";
            if (chatRole == ChatRole.System) return "S";
            if (chatRole == ChatRole.User) return "U";
            throw new Exception($"Unknown value for chat role {chatRole}");

        }

        private ChatRole ChatRoleFromString(string chatRole)
        {
            if (chatRole == "A") return ChatRole.Assistant;
            if (chatRole == "S") return ChatRole.System;
            if (chatRole == "U") return ChatRole.User;
            throw new Exception($"Unknown value for chat role {chatRole}");

        }

        public async Task<List<ConversationItem>> LoadConversation(Guid conversationId)
        {
            var ret = new List<ConversationItem>();
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand($"select id, text,at,chatrole, tokens from conversationItems where conversationHeaderId=@conversationHeaderId order by at ASC");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationHeaderId",
                Value = conversationId,
            });
            cmd.Connection = cn;
            await using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                ret.Add(new ConversationItem
                {
                    ConversationId = conversationId,
                    Id = reader.GetGuid(0),
                    Text = reader.GetString(1),
                    At = reader.GetDateTimeOffset(2),
                    ChatRole = ChatRoleFromString(reader.GetString(3)),
                    Tokens= reader.GetInt32(4),
                });
            }
            return ret;
        }
    }


}
