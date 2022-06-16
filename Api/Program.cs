using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var awsOptions = builder.Configuration.GetAWSOptions();

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    var response = new {Message = "Welcome to chat app"};

    return response;
});

app.MapGet("/chats", async (IDynamoDBContext context, [FromQuery(Name = "session")] string session) =>
{
    ArgumentNullException.ThrowIfNull(session);

    var conditions = new[] {new ScanCondition(nameof(Chat.Session), ScanOperator.Equal, session)};

    var response = await context.ScanAsync<Chat>(conditions).GetRemainingAsync();

    return response;
});

app.MapPost("/chats", async (IDynamoDBContext context, [FromBody] Chat request) =>
{
    var newChat = new Chat(request.Message, request.Session, request.User);

    await context.SaveAsync(newChat);
});

app.Run();
