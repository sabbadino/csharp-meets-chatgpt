using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using myMscChatGpt.Ioc;
using myMscChatGpt.Settings;
using SharpToken;

namespace myMscChatGpt.Services;
public interface IEmbeddingServiceCore
{
    Task<List<float>> GetTextEmbeddings(string text);
}
public class EmbeddingServiceCore : IEmbeddingServiceCore, ISingletonScope
{
    private readonly OpenAIClient _openAiClient;
    private readonly OpenAiSettings _openAiSettings;

    public EmbeddingServiceCore(OpenAIClient openAiClient, IOptions<OpenAiSettings> openAiSettings)
    {
        _openAiClient = openAiClient;
        _openAiSettings = openAiSettings.Value;
    }
    public async Task<List<float>> GetTextEmbeddings(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        var embeddingsOptions = new EmbeddingsOptions(text);

        var response = await _openAiClient.GetEmbeddingsAsync(
            deploymentOrModelName: _openAiSettings.EmbeddingsModel, embeddingsOptions);
        var vector = response.Value.Data[0].Embedding;
        return vector.ToList();

    }
}