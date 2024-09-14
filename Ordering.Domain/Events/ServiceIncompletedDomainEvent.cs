using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record ServiceIncompletedDomainEvent(Order Order) : INotification;
