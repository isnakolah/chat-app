using Amazon.DynamoDBv2.DataModel;

namespace Api.Models;

[DynamoDBTable("chat-api-table")]
public record Chat
{
    [DynamoDBHashKey("id")] public int Id { get; init; }
    [DynamoDBProperty("message")] public string Message { get; init; } = default!;
    [DynamoDBProperty("session")] public string Session { get; init; } = default!;
    [DynamoDBProperty("user")] public string User { get; init; } = default!;
}