using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.LangChain.Bricks;

public class SetApiCallOptionsBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly OpenAiSettings _openAiSettings;

    public SetApiCallOptionsBrick(IOptions<OpenAiSettings> openAi)
    {
        _openAiSettings = openAi.Value;
    }

    public override async Task<AnswerToUser> Ask(Question question)
    {
        question.QuestionOptions.Temperature = _openAiSettings.Temperature;

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        return await Next.Ask(question);
    }

}