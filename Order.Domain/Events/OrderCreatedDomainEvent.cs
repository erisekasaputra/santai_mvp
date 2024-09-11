using MediatR;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Domain.Events;

// Notification to admin or other system
public record OrderCreatedDomainEvent(Ordering Order) : INotification;
