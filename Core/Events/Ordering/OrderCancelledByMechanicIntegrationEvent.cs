using MediatR;

namespace Core.Events.Ordering;

public record OrderCancelledByMechanicIntegrationEvent(
    Guid OrderId, 
    Guid BuyerId, 
    string BuyerName,
    Guid MechanicId, 
    string MechanicName) : INotification;
