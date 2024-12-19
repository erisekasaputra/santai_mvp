using MediatR; 

namespace Ordering.Domain.Events;

public record MechanicArrivedDomainEvent(
    Guid OrderId, 
    Guid BuyerId, 
    Guid MechanicId, 
    string MechanicName ) : INotification;
