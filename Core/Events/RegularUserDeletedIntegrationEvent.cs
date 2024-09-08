using MediatR;

namespace Core.Events;

public record RegularUserDeletedIntegrationEvent(Guid UserId) : INotification;