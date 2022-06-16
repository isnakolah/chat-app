using CLI.Services;
using Shared.DTOs;

var session = GetSessionName();
var userAlias = GetUserAlias();

using var chatService = new ChatService(session, userAlias);

while (true)
{
    try
    {
        Write("Message: ");

        var message = ReadStringFromConsole();

        var chat = new ChatCreateDTO(message, session, userAlias);

        await chatService.SendChat(chat);
    }
    catch (Exception ex)
    {
        WriteLine(ex.Message);
    }
}

static string GetUserAlias()
{
    Write("Enter your name: ");

    return ReadStringFromConsole();
}

static string GetSessionName()
{
    Write("Enter session name: ");

    return ReadStringFromConsole();
}

static string ReadStringFromConsole()
{
    if (ReadLine() is not {Length: > 0} text)
        throw new Exception("No text has been written.");

    return text;
}