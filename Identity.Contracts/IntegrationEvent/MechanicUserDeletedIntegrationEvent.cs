using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record MechanicUserDeletedIntegrationEvent(Guid UserId) : INotification;