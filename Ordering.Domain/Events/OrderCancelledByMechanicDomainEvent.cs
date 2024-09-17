using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

// trigger finding mechanic
public record OrderCancelledByMechanicDomainEvent(Guid OrderId, Guid BuyerId, Guid MechanicId) : INotification;