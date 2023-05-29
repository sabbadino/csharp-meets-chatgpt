using Microsoft.Extensions.Options;
using myMscChatGpt.Dtos.Completition;
using myMscChatGpt.Dtos.Completition.Controllers;
using myMscChatGpt.Ioc;
using myMscChatGpt.Settings;

namespace myMscChatGpt.LangChain.Bricks;

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