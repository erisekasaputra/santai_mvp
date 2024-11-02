namespace Chat.API.Domain.Models;

public class Conversation 
{
    public Guid MessageId { get; set; }
    public Guid OrderId { get; set; }
    public Guid OriginUserId { get; set; }
    public Guid DestinationUserId { get; set; } 
    public string Text { get; set; }
    public string? Attachment { get; set; }
    public Guid? ReplyMessageId { get; set; }
    public string? ReplyMessageText { get; set; }
    public long TimeStamp { get; set; }

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
        TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public Conversation(
        Guid messageId,
        Guid orderId,
        Guid originUserId,
        Guid destinationUserId,
        string text,
        string? attachment,
        Guid? replyMessageId,
        string? replyMessageText,
        long timeStamp)
    {
        MessageId = messageId;
        OrderId = orderId;
        OriginUserId = originUserId;
        DestinationUserId = destinationUserId;
        Text = text;
        Attachment = attachment;
        ReplyMessageId = replyMessageId;
        ReplyMessageText = replyMessageText;
        TimeStamp = timeStamp;
    }
} 
