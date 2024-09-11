using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record OrderRejectedByMechanicDomainEvent(Order Order) : INotification;
