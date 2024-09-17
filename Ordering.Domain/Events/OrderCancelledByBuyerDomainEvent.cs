using MediatR; 

namespace Ordering.Domain.Events;

public record OrderCancelledByBuyerDomainEvent(Guid OrderId, Guid BuyerId, Guid? MechanicId) : INotification;
