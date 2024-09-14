using MediatR;

namespace Core.Events;

public record OrderCancelledByMechanicIntegrationEvent(Guid OrderId, Guid BuyerId, Guid MechanicId) : INotification;
