using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<ChatRepository>();
builder.Services.AddSingleton<IChatRepository, CachedChatRepository>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddMemoryCache();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/chats", async (
    [FromQuery(Name = "session")] string session, [FromQuery(Name = "user")] string user, [FromServices] IChatRepository repository) =>
{
    return await repository.GetAllAsync(chat => chat.Session == session && chat.User != user);
});

app.MapPost("/chats", async ([FromBody] ChatCreateDTO newChatDto, [FromServices] IChatRepository repository) =>
{
    var newChat = new Chat
    {
        Message = newChatDto.Message,
        Session = newChatDto.Session,
        User = newChatDto.User
    };

    await repository.AddAsync(newChat);
});

app.Run();
