using MediatR;

namespace Chat.API.Domain.Events;

public record ChatSentDomainEvent(
    Guid OrderId,
    Guid OriginUserId,
    Guid DestinationUserId,
    Guid MessageId,
    string Text,
    string Attachment,
    Guid ReplyMessageId,
    string ReplyMessageText,
    long Timestamp) : INotification;
