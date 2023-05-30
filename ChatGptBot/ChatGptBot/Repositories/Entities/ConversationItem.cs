using Azure.AI.OpenAI;

namespace ChatGptBot.Repositories.Entities;

public class ConversationItem
{
    public required Guid ConversationId { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Text { get; init; } = "";

    public required int Tokens { get; init; } 

    public required ChatRole ChatRole { get; init; }

    public DateTimeOffset At { get; init; } = DateTimeOffset.UtcNow;
}