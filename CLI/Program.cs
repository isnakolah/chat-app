using CLI.Services;
using Shared.DTOs;

const string session = "new-fire";
const string userAlias = "isnakolah";

using var chatService = new ChatService(session);

static string GetMessageFromConsole()
{
    Write("Message: ");

    if (ReadLine() is not {Length: > 0} message)
        throw new Exception("No message has been written.");

    return message;
}

while (true)
{
    try
    {
        var message = GetMessageFromConsole();

        var chat = new ChatCreateDTO(message, session, userAlias);

        await chatService.SendChat(chat);
    }
    catch (Exception ex)
    {
        WriteLine(ex.Message);
    }
}