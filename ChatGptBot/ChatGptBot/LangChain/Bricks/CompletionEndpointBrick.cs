using Azure.AI.OpenAI;
using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.LangChain.Bricks;

public class CompletionEndpointBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly OpenAIClient _openAiClient;
    private readonly OpenAiSettings _openAiSettingsSettings;

    public CompletionEndpointBrick(OpenAIClient openAiClient, IOptions<OpenAiSettings> openAiSettingsSettings)
    {
        _openAiClient = openAiClient;
        _openAiSettingsSettings = openAiSettingsSettings.Value;
    }
    public override async Task<AnswerToUser> Ask(Question question)
    {
        if (string.IsNullOrEmpty(question.UserQuestion.Text))
        {
            throw new Exception($"{nameof(UserQuestion)} is null");
        }

        var chatCompletionsOptions = new ChatCompletionsOptions();
        question.SystemMessages.ForEach(systemMessage => chatCompletionsOptions.Messages.Add(new 
            ChatMessage (ChatRole.System, systemMessage.Text)));

        question.ContentMessages.ForEach(contentMessage=> chatCompletionsOptions.Messages.Add(new
            ChatMessage(ChatRole.User, contentMessage.Text)));

        question.ConversationHistoryMessages.ForEach(conversationItem=> chatCompletionsOptions.Messages.Add(new
            ChatMessage(conversationItem.ChatRole, conversationItem.textWIthTokenCount.Text)));

        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, question.UserQuestion.Text));

        chatCompletionsOptions.Temperature = question.QuestionOptions.Temperature;


        var response = await _openAiClient.GetChatCompletionsAsync(
            deploymentOrModelName: _openAiSettingsSettings.ModelName ,
            chatCompletionsOptions);
        var completion = response.Value.Choices[0].Message;

        return new AnswerToUser { AnswerFromAi = completion.Content };

    }

    

}

