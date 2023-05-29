namespace myMscChatGpt.Dtos.Completition.Controllers;

public class AnswerToUser
{
    public string AnswerFromAi { get; set; } = "";
    public Guid ConversationId { get; set; } = Guid.Empty;
}