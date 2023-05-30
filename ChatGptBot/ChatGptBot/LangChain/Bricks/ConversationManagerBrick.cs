using Azure.AI.OpenAI;
using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.Repositories;
using ChatGptBot.Repositories.Entities;
using SharpToken;

namespace ChatGptBot.LangChain.Bricks;

public class ConversationManagerBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly IConversationRepository _conversationRepository;
    private readonly GptEncoding _gptEncoding;

    public ConversationManagerBrick(IConversationRepository conversationRepository, GptEncoding gptEncoding)
    {
        _conversationRepository = conversationRepository;
        _gptEncoding = gptEncoding;
    }

    public override async Task<AnswerToUser> Ask(Question question)
    {

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }

        var questionConversationItem = new ConversationItem
        {
            Text = question.UserQuestion.Text,
            ChatRole = ChatRole.User,
            ConversationId = question.ConversationId,
            Tokens = question.UserQuestion.Tokens
        };


        if (question.ConversationId != Guid.Empty)
        {
            var items = await _conversationRepository.LoadConversation(question.ConversationId);
            items.ForEach(i => question.ConversationHistoryMessages.Add((i.ChatRole, new TextWIthTokenCount { Text= i.Text, Tokens =i.Tokens})));
        }

        var ret = await Next.Ask(question);
        if (question.ConversationId != Guid.Empty)
        {
            ret.ConversationId = question.ConversationId;   
            if (question.ConversationId != Guid.Empty)
            {
                await _conversationRepository.StoreConversationItem(questionConversationItem);
                await _conversationRepository.StoreConversationItem(
                    new ConversationItem
                    {
                        ConversationId = question.ConversationId,
                        Text = ret.AnswerFromAi,
                        ChatRole = ChatRole.Assistant,
                        At = DateTimeOffset.UtcNow,
                        Tokens = _gptEncoding.Encode(ret.AnswerFromAi).Count
                    });
            }
        }
        return ret;
    }
}