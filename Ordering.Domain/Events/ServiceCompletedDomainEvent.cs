using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record ServiceCompletedDomainEvent(Order Order) : INotification;
