using Amazon.DynamoDBv2.DataModel;

namespace Api.Models;

[DynamoDBTable("chat-api-table")]
public record Chat
{
    public Chat()
    {
    }
    
    public Chat(string message, string session, string user)
    {
        (Message, Session, User) = (message, session, user);
    }

    [DynamoDBHashKey("created-on")] public DateTime CreatedOn { get; init; } = DateTime.Now;
    [DynamoDBProperty("message")] public string Message { get; init; } = default!;
    [DynamoDBProperty("session")] public string Session { get; init; } = default!;
    [DynamoDBProperty("user")] public string User { get; init; } = default!;
}