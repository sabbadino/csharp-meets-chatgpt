using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using myMscChatGpt.Dtos.Completition;
using myMscChatGpt.Dtos.Completition.Controllers;
using myMscChatGpt.Ioc;
using myMscChatGpt.Services.PlaceHolders;
using myMscChatGpt.Settings;
using System.Text;

namespace myMscChatGpt.LangChain.Bricks;

public class PlaceHolderResolverBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly IPlaceHolderResolverService _placeHolderResolverService;

    public PlaceHolderResolverBrick(IPlaceHolderResolverService placeHolderResolverService)
    {
        _placeHolderResolverService = placeHolderResolverService;
    }
    public override async Task<AnswerToUser> Ask(Question question)
    {
        if (string.IsNullOrEmpty(question.UserQuestion.Text))
        {
            throw new Exception($"{nameof(UserQuestion)} is null");
        }

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        var response =  await Next.Ask(question);
        response.AnswerFromAi = _placeHolderResolverService.ReplacePlaceHolders(response.AnswerFromAi);

        return response;
    }
}