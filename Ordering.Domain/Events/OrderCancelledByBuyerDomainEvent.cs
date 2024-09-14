using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record OrderCancelledByBuyerDomainEvent(Order Order) : INotification;
