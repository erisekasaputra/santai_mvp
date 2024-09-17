using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record OrderRatedDomainEvent(
    Guid OrderId,
    Guid BuyerId,
    decimal Value,
    string? Comment) : INotification;
