using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatGptBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompletionController : ControllerBase
    {
        private readonly ICompletionService _completionService;


        public CompletionController(ICompletionService completionService)
        {
            _completionService = completionService;
        }

        [HttpPost("ask",Name = "Ask")]
        public async Task<AnswerToUser> Ask(UserQuestion userQuestion)
        {
            
            return await _completionService.Ask(userQuestion);  

        }

        [HttpPost("dumb-ask",Name = "DumbAsk")]
        public async Task<AnswerToUser> DumbAsk(UserQuestion userQuestion)
        {

            return await _completionService.DumbAsk(userQuestion);

        }
    }
}