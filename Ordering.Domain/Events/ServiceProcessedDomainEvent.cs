using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record ServiceProcessedDomainEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
