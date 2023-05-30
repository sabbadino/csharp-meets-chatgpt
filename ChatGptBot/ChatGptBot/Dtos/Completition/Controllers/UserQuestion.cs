namespace ChatGptBot.Dtos.Completition.Controllers;

public class UserQuestion
{
    public Guid ConversationId { get; init; } = Guid.NewGuid();
    public string QuestionText { get; init; } = "";
}