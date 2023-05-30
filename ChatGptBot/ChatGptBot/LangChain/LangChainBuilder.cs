using ChatGptBot.Ioc;

namespace ChatGptBot.LangChain
{
    public interface ILangChainBuilder
    {
        LangChainBuilder Add(ILangChainBrick brick);
        ILangChainBrick Build();
    }
    public class LangChainBuilder : ILangChainBuilder, ISingletonScope
    {
        private ILangChainBrick?  _current ;
        private ILangChainBrick? _first;

        public LangChainBuilder Add(ILangChainBrick brickBase)
        {
            if (_current == null)
            {
                _first = brickBase;
            }
            else
            {
                _current.SetNext(brickBase);
            }

            _current = brickBase;    
            return this;
        }

        public ILangChainBrick Build()
        {
            if (_first == null)
            {
                throw new Exception("no LangChainItemBase provided");
            }
            return _first;
        }
    }
}
