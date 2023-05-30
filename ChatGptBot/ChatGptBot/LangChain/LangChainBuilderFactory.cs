using ChatGptBot.Ioc;

namespace ChatGptBot.LangChain;
public interface ILangChainBuilderFactory
{
    ILangChainBuilder Create();
}
public class LangChainBuilderFactory : ILangChainBuilderFactory, ISingletonScope
{
    public ILangChainBuilder Create() { return  new LangChainBuilder(); }
}