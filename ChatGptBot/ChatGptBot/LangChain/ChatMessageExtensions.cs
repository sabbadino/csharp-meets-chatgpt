using ChatGptBot.Dtos.Completition;

namespace ChatGptBot.LangChain;

public static class ChatMessageExtensions
{
    public static int TotalTokensInQuestion(this Question question)
    {
        var totalInputTokens = 0;
        totalInputTokens = question.SystemMessages.Aggregate(totalInputTokens, (counter, item) => counter + item.Tokens);
        totalInputTokens = question.ConversationHistoryMessages.Aggregate(totalInputTokens, (counter, item) => counter + item.textWIthTokenCount.Tokens);
        totalInputTokens = question.ContentMessages.Aggregate(totalInputTokens, (counter, item) => counter + item.Tokens);
        totalInputTokens += question.UserQuestion.Tokens;
        return totalInputTokens;

    }




}