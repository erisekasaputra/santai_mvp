using MediatR;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Domain.Events;

// trigger finding mechanic
public record OrderRejectedDomainEvent(Ordering Order) : INotification;
