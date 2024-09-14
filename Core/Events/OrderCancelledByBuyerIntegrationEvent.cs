using MediatR;

namespace Core.Events;

public record OrderCancelledByBuyerIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid? MechanicId) : INotification;
