using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Api.Common;

namespace Api.Models;

[DynamoDBTable("chat-api-table")]
public record Chat
{
    [JsonIgnore]
    [DynamoDBHashKey("timestamp")]
    public long Timestamp { get; init; } = DateTime.Now.DateTimeToUnixTimestamp();

    [DynamoDBProperty("message")]
    public string Message { get; init; } = default!;

    [DynamoDBProperty("session")]
    public string Session { get; init; } = default!;

    [DynamoDBProperty("user")]
    public string User { get; init; } = default!;

    public DateTime CreatedOn => Timestamp.UnixTimeStampToDateTime();
}