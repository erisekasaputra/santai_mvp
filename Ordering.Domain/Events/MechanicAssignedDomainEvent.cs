using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record MechanicAssignedDomainEvent(
    Guid OrderId , 
    Guid BuyerId, 
    Guid MechanicId,
    string MechanicName) : INotification;
