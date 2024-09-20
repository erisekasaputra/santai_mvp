using MediatR; 

namespace Ordering.Domain.Events;

// trigger finding mechanic
public record OrderCancelledByMechanicDomainEvent(
    Guid OrderId, 
    Guid BuyerId, 
    string BuyerName,
    Guid MechanicId,
    string MechanicName) : INotification;