using MediatR;

namespace Ordering.Domain.Events;

public record MechanicDispatchedDomainEvent(
    Guid OrderId, 
    Guid BuyerId, 
    Guid MechanicId,
    string MechanicName) : INotification;
