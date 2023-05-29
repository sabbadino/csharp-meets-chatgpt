﻿using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using myMscChatGpt.Dtos.Completition;
using myMscChatGpt.Dtos.Completition.Controllers;
using myMscChatGpt.Ioc;
using myMscChatGpt.Repositories;
using myMscChatGpt.Settings;
using SharpToken;

namespace myMscChatGpt.LangChain.Bricks;

public class QuestionLengthGuardBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly GptEncoding _gptEncoding;
    private readonly OpenAiSettings _openAiSettings;
  

    public QuestionLengthGuardBrick(IOptions<OpenAiSettings> openAiSettings, GptEncoding gptEncoding)
    {
        _gptEncoding = gptEncoding;
        _openAiSettings = openAiSettings.Value;
    }

    public override async Task<AnswerToUser> Ask(Question question)
    {
        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        if (question.UserQuestion.Tokens > _openAiSettings.TokenRatioForUserQuestion)
        {
            throw new Exception($"Please write your question using less words");
        }
        return await Next.Ask(question);
    }

}