using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

// Notification to admin or other system
public record OrderCreatedDomainEvent(Order Order) : INotification;
