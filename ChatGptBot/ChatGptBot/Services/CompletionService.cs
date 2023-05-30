using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.LangChain;
using ChatGptBot.LangChain.Bricks;
using SharpToken;

namespace ChatGptBot.Services
{
    public interface ICompletionService
    {
        Task<AnswerToUser> Ask(UserQuestion question);
        Task<AnswerToUser> DumbAsk(UserQuestion userQuestion);
    }

    public class CompletionService : ICompletionService, ISingletonScope
    {
        private readonly IEnumerable<ILangChainBrick> _langChainBricks;
        private readonly GptEncoding _gptEncoding;
        private readonly ILangChainBuilderFactory _langChainBuilderFactory;
     
        public CompletionService(ILangChainBuilderFactory langChainBuilderFactory, 
            IEnumerable<ILangChainBrick> langChainBricks, GptEncoding gptEncoding)
        {
            _langChainBricks = langChainBricks;
            _gptEncoding = gptEncoding;
            _langChainBuilderFactory = langChainBuilderFactory;
        }

        public async Task<AnswerToUser> DumbAsk(UserQuestion userQuestion)
        {
            var builder = _langChainBuilderFactory.Create();
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(QuestionLengthGuardBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));

            var langChainITem = builder.Build();
            var question = new Question
            {
                ConversationId = userQuestion.ConversationId,
                UserQuestion = new TextWIthTokenCount
                {
                    Text = $"Question: {userQuestion.QuestionText}"
                    ,
                    Tokens = _gptEncoding.Encode(userQuestion.QuestionText).Count
                }
            };
            return await langChainITem.Ask(question);
        }

        public async Task<AnswerToUser> Ask(UserQuestion userQuestion)
        {
            var builder = _langChainBuilderFactory.Create();
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(QuestionLengthGuardBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(ConversationManagerBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(SetSystemMessageBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(SetContextBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(MaxTokenGuardBrick))); 
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            builder.Add(_langChainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));

            var langChainITem = builder.Build();
            var question = new Question
            {
                ConversationId = userQuestion.ConversationId,
                UserQuestion = new TextWIthTokenCount { Text = $"Question: {userQuestion.QuestionText}"
                    , Tokens = _gptEncoding.Encode(userQuestion.QuestionText).Count
                }
            };
            return await langChainITem.Ask(question);
        }
    }
}
