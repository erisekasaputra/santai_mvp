using MediatR;

namespace Chat.API.Domain.Events;

public record ChatSentDomainEvent(Guid OriginUserId, Guid DestinationUserId, Guid MessageId, string Text, long Timestamp) : INotification;
