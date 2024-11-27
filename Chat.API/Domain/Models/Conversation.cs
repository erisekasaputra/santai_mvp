using Amazon.DynamoDBv2.DataModel;

namespace Chat.API.Domain.Models;

[DynamoDBTable("Conversation")] 
public class Conversation
{
    [DynamoDBHashKey] // Partition Key
    public string MessageId { get; init; }
    public string OrderId { get; init; }
    public string OriginUserId { get; init; }
    public string DestinationUserId { get; init; }
    public string Text { get; set; }
    public string? Attachment { get; set; }
    public string? ReplyMessageId { get; set; }
    public string? ReplyMessageText { get; set; }
    [DynamoDBRangeKey] // Sort Key
    public long Timestamp { get; init; }
    [DynamoDBVersion]
    public int? Version { get; set; }
    public Conversation()
    {
        MessageId = string.Empty;
        OrderId = string.Empty;
        OriginUserId = string.Empty;
        DestinationUserId = string.Empty;  
        Text = string.Empty;
    }
    public Conversation(
        string orderId,
        string originUserId,
        string destinationUserId,
        string text,
        string? attachment,
        string? replyMessageId,
        string? replyMessageText)
    {
        MessageId = Guid.NewGuid().ToString();
        OrderId = orderId;
        OriginUserId = originUserId;
        DestinationUserId = destinationUserId;
        Text = text;
        Attachment = attachment;
        ReplyMessageId = replyMessageId;
        ReplyMessageText = replyMessageText;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    } 
}