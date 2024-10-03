using MediatR;

namespace Core.Events.Chat;

public record ChatSentIntegrationEvent(Guid OriginUserId, Guid DestinationUserId, Guid MessageId, string Text, long Timestamp) : INotification;
