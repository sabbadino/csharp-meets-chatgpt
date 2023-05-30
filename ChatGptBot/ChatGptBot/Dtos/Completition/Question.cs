using Azure.AI.OpenAI;
using ChatGptBot.Dtos.Completition.Controllers;

namespace ChatGptBot.Dtos.Completition;

public class TextWIthTokenCount
{
    public required string Text  { get; init; }
    public required int Tokens { get; init; }
}

public class Question
{
    public string? ModelName { get; init; }
    public string? EmbeddingSetCode { get; init; }
    public Guid ConversationId { get; init; }
    public List<TextWIthTokenCount> SystemMessages { get; init; } = new();

    public List<TextWIthTokenCount> ContentMessages { get; init; } = new();

    public List<(ChatRole ChatRole, TextWIthTokenCount textWIthTokenCount)> ConversationHistoryMessages { get; init; } = new();

    public TextWIthTokenCount UserQuestion { get; init; } = new TextWIthTokenCount {Text="",Tokens=0 };

    public QuestionOptions QuestionOptions { get; } = new();
    public int? EmbeddingMatchMaxItems { get; set; }
    
}