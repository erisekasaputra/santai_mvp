using MediatR;

namespace Core.Events;

public record ServiceIncompletedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
