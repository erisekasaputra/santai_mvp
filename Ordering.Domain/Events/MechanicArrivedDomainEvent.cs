using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record MechanicArrivedDomainEvent(Guid OrderId, Guid BuyerId, Guid MechanicId) : INotification;
