using Azure.AI.OpenAI;
using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.Repositories;
using ChatGptBot.Services;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;
using SharpToken;
using ICosineProximityService = ChatGptBot.Services.ICosineProximityService;
using IEmbeddingServiceCore = ChatGptBot.Services.IEmbeddingServiceCore;

namespace ChatGptBot.LangChain.Bricks;

public class SetContextBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly ICosineProximityService _cosineProximityService;
    private readonly IEmbeddingRepositoryCache _embeddingRepositoryCache;
    private readonly IEmbeddingServiceCore _embeddingServiceCore;
    private readonly GptEncoding _gptEncoding;
    private readonly OpenAiSettings _openAiSettings;

       public SetContextBrick(ICosineProximityService cosineProximityService, IEmbeddingRepositoryCache embeddingRepositoryCache
    , IOptions<OpenAiSettings> openAi, IEmbeddingServiceCore embeddingServiceCore, GptEncoding gptEncoding)
    {
        _openAiSettings= openAi.Value; 
        _cosineProximityService = cosineProximityService;
        _embeddingRepositoryCache = embeddingRepositoryCache;
        _embeddingServiceCore = embeddingServiceCore;
        _gptEncoding = gptEncoding;
    }

    public override async Task<AnswerToUser> Ask(Question question)
    {
        if (question.ConversationId != Guid.Empty)
        {
            var maxItems = question.EmbeddingMatchMaxItems ?? _openAiSettings.DefaultEmbeddingMatchMaxItems;
            var set = await _embeddingRepositoryCache.LoadSet(question.EmbeddingSetCode ??
                                                              _openAiSettings.DefaultEmbeddingSetCode);
            var allUserQuestions = new List<string> {question.UserQuestion.Text};
            allUserQuestions.AddRange(question.ConversationHistoryMessages.Where(c => c.ChatRole == ChatRole.User).Select(c=> c.textWIthTokenCount.Text));
            var closeEmbeddings = new List<(Services.Embedding Embedding, float Proximity)>();
            foreach (var userQuestion in allUserQuestions)
            {
                closeEmbeddings.AddRange(_cosineProximityService.GetClosestMatches(new ProximityRequest
                {
                    InputEmbedding = await _embeddingServiceCore.GetTextEmbeddings(userQuestion),
                    EmbeddingSet = set,
                    MaxItems = maxItems
                }).ToList());
            }
            closeEmbeddings = closeEmbeddings.DistinctBy(i=> i.Embedding.Id).OrderByDescending( i=> i.Proximity).Take(maxItems).ToList();
            if (closeEmbeddings.Any())
            {
                question.ContentMessages.Add(new TextWIthTokenCount
                {
                    Text = "Answer to the question using the following information:",
                    Tokens = _gptEncoding.Encode("Answer to the question using the following information:").Count
                });
                closeEmbeddings.ForEach(t =>
                    question.ContentMessages.Add(new TextWIthTokenCount {Text = t.Embedding.Text, Tokens = t.Embedding.Tokens}));
            }
        }

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        return await Next.Ask(question);
    }

}