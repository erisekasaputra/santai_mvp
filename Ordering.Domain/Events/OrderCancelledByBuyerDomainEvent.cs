using MediatR; 

namespace Ordering.Domain.Events;

public record OrderCancelledByBuyerDomainEvent(
    Guid OrderId, 
    Guid BuyerId, 
    string BuyerName,
    Guid? MechanicId,
    string MechanicName) : INotification;
