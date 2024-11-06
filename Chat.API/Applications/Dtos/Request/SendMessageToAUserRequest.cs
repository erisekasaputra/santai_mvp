namespace Chat.API.Applications.Dtos.Request;

public record SendMessageRequest(
    string OrderId,
    string DestinationUserId,
    string Text,
    string Attachment,
    string ReplyMessageId,
    string ReplyMessageText);
