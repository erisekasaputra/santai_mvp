 

namespace Chat.API.Applications.Dtos.Response;

public class ConversationResponse
{
    public string MessageId { get; init; }
    public string OrderId { get; init; }
    public string OriginUserId { get; init; }
    public string DestinationUserId { get; init; }
    public string Text { get; init; }
    public string Attachment { get; init; }
    public string ReplyMessageId { get; init; }
    public string ReplyMessageText { get; init; }
    public long Timestamp { get; init; } 
    public ConversationResponse(
        string messageId,
        string orderId,
        string originUserId,
        string destinationUserId,
        string text,
        string attachment,
        string replyMessageId,
        string replyMessageText,
        long timestamp)
    {
        MessageId = messageId;
        OrderId = orderId;
        OriginUserId = originUserId;
        DestinationUserId = destinationUserId;
        Text = text;
        Attachment = attachment;
        ReplyMessageId = replyMessageId;
        ReplyMessageText = replyMessageText;
        Timestamp = timestamp;
    }
}
