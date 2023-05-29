using myMscChatGpt.Dtos.Completition;
using myMscChatGpt.Dtos.Completition.Controllers;

namespace myMscChatGpt.LangChain;

public interface ILangChainBrick
{
    void SetNext(ILangChainBrick langChainBrickBase);
    
    public ILangChainBrick? Next { get; }

    public abstract Task<AnswerToUser> Ask(Question question);
}

public abstract class LangChainBrickBase : ILangChainBrick
{
    public void SetNext(ILangChainBrick langChainBrickBase)
    {
        Next = langChainBrickBase;
    }

    public ILangChainBrick? Next { get; private set; }

    public abstract Task<AnswerToUser> Ask(Question question);

}