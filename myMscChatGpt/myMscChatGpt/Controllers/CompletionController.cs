using AutoMapper;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using myMscChatGpt.Dtos.Completition.Controllers;
using myMscChatGpt.LangChain;
using myMscChatGpt.Repositories;
using myMscChatGpt.Services;

namespace myMscChatGpt.Controllers
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