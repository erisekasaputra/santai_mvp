using MediatR;

namespace Core.Events.Ordering;

public record ServiceIncompletedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
