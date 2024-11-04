using MediatR;

namespace Core.Events.Chat;

public record ChatSentIntegrationEvent(
    Guid MessageId,
    Guid OrderId,
    Guid OriginUserId,
    Guid DestinationUserId,
    string Text,
    string? Attachment,
    Guid? ReplyMessageId,
    string? ReplyMessageText,
    long Timestamp) : INotification;
