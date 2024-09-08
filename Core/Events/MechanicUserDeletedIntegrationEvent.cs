using MediatR;

namespace Core.Events;

public record MechanicUserDeletedIntegrationEvent(Guid UserId) : INotification;