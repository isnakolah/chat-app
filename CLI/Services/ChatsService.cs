using System.Net.Http.Json;
using Shared.DTOs;

namespace CLI.Services;

public class ChatService : IDisposable
{
    private readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:5001")
    };

    public event EventHandler<IEnumerable<ChatGetDTO>> ChatsReceived;

    private void OnChatsReceived(IEnumerable<ChatGetDTO> chats)
    {
        ChatsReceived.Invoke(this, chats);
    }

    public ChatService(string session)
    {
        ChatsReceived += c_ChatsReceived!;

        GetChatsWithEvents(session).ConfigureAwait(false);
    }

    private static void c_ChatsReceived(object sender, IEnumerable<ChatGetDTO> chats)
    {
        var enumeratedChats = chats as ChatGetDTO[] ?? chats.ToArray();

        var chatsIsValid = enumeratedChats.Length > 0;

        if (chatsIsValid)
            WriteLine();

        foreach (var chat in enumeratedChats)
            WriteLine($"{chat.User}: {chat.Message}");

        if (chatsIsValid)
            Write("\nMessage: ");
    }

    public async Task SendChat(ChatCreateDTO chat)
    {
        await httpClient.PostAsJsonAsync("chats", chat);
    }

    private async Task<IEnumerable<ChatGetDTO>> GetChats(string session)
    {
        var result = await httpClient.GetFromJsonAsync<ChatGetDTO[]>($"chats?session={session}");

        return result ?? Enumerable.Empty<ChatGetDTO>();
    }

    private async Task GetChatsWithEvents(string session)
    {
        while (true)
        {
            await Task.Delay(10000);

            var chats = await GetChats(session);

            OnChatsReceived(chats);
        }
    }

    public void Dispose()
    {
        httpClient.Dispose();

        ChatsReceived -= c_ChatsReceived!;
    }
}