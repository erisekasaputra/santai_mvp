using MediatR;

namespace Core.Events;

public record OrderMechanicArrivedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
