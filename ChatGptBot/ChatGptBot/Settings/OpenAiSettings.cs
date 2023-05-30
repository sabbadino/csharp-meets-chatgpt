namespace ChatGptBot.Settings
{
    public class OpenAiSettings
    {
        public string ModelName { get; init; } = "gpt-3.5-turbo";

        public string EmbeddingsModel { get; init; } = "text-embedding-ada-002";

        public float SimilarityThreshold { get; init; } = 0.75f;
        
        public string SystemMessage { get; init; } = "";
        public float Temperature { get; init; } = 0.3f;
        public string DefaultEmbeddingSetCode { get; init; } = "test1";
        public int DefaultEmbeddingMatchMaxItems { get; init; } = 3;

        public int MaxTokens { get; init; } = 4096;


        public int MinimumAvailableTokensForTokenForAnswer => 50;

        public float TokenRatioForUserQuestion => Convert.ToInt32(MaxTokens * 0.05f);
    }
}
