using Microsoft.Extensions.Options;
using myMscChatGpt.Dtos.Completition;
using myMscChatGpt.Dtos.Completition.Controllers;
using myMscChatGpt.Ioc;
using myMscChatGpt.Settings;
using SharpToken;
using System.Runtime.CompilerServices;

namespace myMscChatGpt.LangChain.Bricks;

public class MaxTokenGuardBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly GptEncoding _gptEncoding;
    private readonly OpenAiSettings _openAiSettings;


    public MaxTokenGuardBrick(IOptions<OpenAiSettings> openAi, GptEncoding gptEncoding)
    {
        _gptEncoding = gptEncoding;
        _openAiSettings = openAi.Value;
    }

    public override async Task<AnswerToUser> Ask(Question question)
    {
        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }

        var totalTokensInQuestion = question.TotalTokensInQuestion();
        while (true)
        {
            // there are 50 tokens left for the answer 
            // chatpgpt says he is capable to adjust himself to adapt answer size to token max token limit
            if ((totalTokensInQuestion + _openAiSettings.MinimumAvailableTokensForTokenForAnswer) <= _openAiSettings.MaxTokens)
            {
                break;
            }

            if (question.ConversationHistoryMessages.Count > 6)
            {
                // remove older conversation items pair , better to loose conversation context than content messages
                question.ConversationHistoryMessages.RemoveAt(0);
                question.ConversationHistoryMessages.RemoveAt(0);
            }
            else
            {
                // most relevant were put on top 
                question.ContentMessages.RemoveAt(question.ContentMessages.Count-1);
            }
            totalTokensInQuestion = question.TotalTokensInQuestion();
        }
        return await Next.Ask(question);
    }
}