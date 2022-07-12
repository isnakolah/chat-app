using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Shared.DTOs;

namespace CLI.Services;

public class ChatService : IDisposable
{
    private DateTime LastCreatedOn { get; set; }
    private string UserAlias { get; set; }
    private string Session { get; set; }

    private readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("https://1rur03flf4.execute-api.us-east-1.amazonaws.com/Prod/")
    };

    public event EventHandler<IEnumerable<ChatGetDTO>> ChatsReceived;

    private void OnChatsReceived(IEnumerable<ChatGetDTO> chats)
    {
        ChatsReceived.Invoke(this, chats);
    }

    public ChatService(string session, string userName)
    {
        ChatsReceived += c_ChatsReceived!;

        (UserAlias, Session) = (userName, session);

        GetChatsWithEvents().ConfigureAwait(false);
    }

    private void c_ChatsReceived(object sender, IEnumerable<ChatGetDTO> chats)
    {
        var orderedChats = (chats as ChatGetDTO[] ?? chats.ToArray())
            .Where(chat => chat.CreatedOn > LastCreatedOn)
            .OrderBy(chat => chat.CreatedOn)
            .ToArray();

        if (orderedChats.Length == 0) return;

        LastCreatedOn = orderedChats.Last().CreatedOn;

        WriteLine();

        foreach (var chat in orderedChats)
            WriteLine($"... [{chat.CreatedOn.ToShortTimeString()}] {chat.User}: {chat.Message}");

        Write("\nMessage: ");
    }

    public async Task SendChat(ChatCreateDTO chat)
    {
        await httpClient.PostAsJsonAsync("chats", chat);
    }

    private async Task<IEnumerable<ChatGetDTO>> GetChats()
    {
        var result = await httpClient.GetFromJsonAsync<ChatGetDTO[]>($"chats?session={Session}&user={UserAlias}");

        return result ?? Enumerable.Empty<ChatGetDTO>();
    }

    private async Task GetChatsWithEvents()
    {
        while (true)
        {
            await Task.Delay(3000);

            var chats = await GetChats();

            OnChatsReceived(chats);
        }
    }

    public void Dispose()
    {
        httpClient.Dispose();

        ChatsReceived -= c_ChatsReceived!;
    }
}