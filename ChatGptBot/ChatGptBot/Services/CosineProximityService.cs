using System.Collections.Concurrent;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Services
{
    public interface ICosineProximityService
    {
        IEnumerable<(Embedding, float Proximity)> GetClosestMatches(ProximityRequest proximityRequest);
    }


    public class ProximityRequest
    {
        public List<float> InputEmbedding { get; init; } = new();
        public List<Embedding> EmbeddingSet { get; init; } = new();

        public int MaxItems { get; init; } = 1;

        public float? SimilarityThreshold { get; init; } 
        

    }

    public class CosineProximityService : ICosineProximityService, ISingletonScope
    {
        private readonly OpenAiSettings _openAiSettings;

        public CosineProximityService(IOptions<OpenAiSettings> openAiSettings)
        {
            _openAiSettings = openAiSettings.Value;
        }
        public IEnumerable<(Embedding, float Proximity)> GetClosestMatches(ProximityRequest proximityRequest)
        {
            var result = new ConcurrentBag<(Embedding embedding, float cosine)>();

            Parallel.ForEach(proximityRequest.EmbeddingSet, embedding =>
            {
                var proximity = GetProximity(embedding.VectorValues, proximityRequest.InputEmbedding);
                result.Add((embedding, proximity));
            });
            var similarityThreshold = proximityRequest.SimilarityThreshold ?? _openAiSettings.SimilarityThreshold;
            var ret =  result.OrderByDescending(item => item.cosine)
                .Where(item => item.cosine > similarityThreshold)   
                .Take(proximityRequest.MaxItems)
                .Select(item => (item.embedding, item.cosine));
            return ret;
        }
       

        public float GetProximity(List<float> item1, List<float> item2)
        {
           
            var vectorLength = item1.Count;
            var sumCosine = 0.0f;
            var item1Length = 0.0f;
            var item2Length = 0.0f;
            for (var i = 0; i < vectorLength; i++)
            {
                item1Length += Convert.ToSingle(Math.Pow(item1[i], 2));
                item2Length += Convert.ToSingle(Math.Pow(item2[i], 2));
                sumCosine += item1[i] * item2[i];
            }
            var proximity = sumCosine / (Math.Pow(item1Length, 0.5) * Math.Pow(item2Length, 0.5));
            return Convert.ToSingle(proximity);
        }

    }

   

    
}
