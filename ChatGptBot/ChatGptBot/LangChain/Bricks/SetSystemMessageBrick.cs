using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;
using SharpToken;

namespace ChatGptBot.LangChain.Bricks;

public class SetSystemMessageBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly GptEncoding _gptEncoding;

    private readonly OpenAiSettings _openAiSettings;

    public SetSystemMessageBrick(IOptions<OpenAiSettings> openAi, GptEncoding gptEncoding)
    {
        _gptEncoding = gptEncoding;
        _openAiSettings = openAi.Value;
    }

    public override async Task<AnswerToUser> Ask(Question question)
    {
        var systemMessage = _openAiSettings.SystemMessage;
        if (!string.IsNullOrEmpty(systemMessage))
        {
            question.SystemMessages.Add(new TextWIthTokenCount
                {Text = systemMessage, Tokens = _gptEncoding.Encode(systemMessage).Count});
        }

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        return await Next.Ask(question);
    }

}