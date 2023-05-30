using Azure.AI.OpenAI;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.AspNetCore.Hosting;
using SharpToken;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var openAiApiKey = builder.Configuration[$"{nameof(OpenAiSettings)}:apiKey"];
var client = new OpenAIClient(openAiApiKey, new OpenAIClientOptions(OpenAIClientOptions.ServiceVersion.V2023_03_15_Preview));
builder.Services.AddSingleton(client);
builder.Services.RegisterByConvention<Program>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddOptions<OpenAiSettings>().Bind(builder.Configuration.GetSection(nameof(OpenAiSettings)));
builder.Services.AddOptions<Storage>().Bind(builder.Configuration.GetSection(nameof(Storage)));
builder.Services.AddOptions<PlaceholdersInformation>().Bind(builder.Configuration.GetSection(nameof(PlaceholdersInformation)));

var tikTokenEncoding = builder.Configuration[$"{nameof(OpenAiSettings)}:tikToken"];
var encoding = GptEncoding.GetEncoding(tikTokenEncoding);
builder.Services.AddSingleton(encoding);
builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
