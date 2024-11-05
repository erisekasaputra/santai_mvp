using Amazon.DynamoDBv2.DataModel;

namespace Chat.API.Domain.Models;

[DynamoDBTable("Conversation")] 
public class Conversation
{
    [DynamoDBHashKey] // Partition Key
    public Guid MessageId { get; init; }
    public Guid OrderId { get; init; }
    public Guid OriginUserId { get; init; }
    public Guid DestinationUserId { get; init; }
    public string Text { get; set; }
    public string? Attachment { get; set; }
    public Guid? ReplyMessageId { get; set; }
    public string? ReplyMessageText { get; set; }
    [DynamoDBRangeKey] // Sort Key
    public long Timestamp { get; init; }

    public Conversation()
    {
        Text = string.Empty;
    }
    public Conversation(
        Guid orderId,
        Guid originUserId,
        Guid destinationUserId,
        string text,
        string? attachment,
        Guid? replyMessageId,
        string? replyMessageText)
    {
        MessageId = Guid.NewGuid();
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