using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record MechanicArrivedDomainEvent(Order Order) : INotification;
