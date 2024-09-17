using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record OrderFindingMechanicDomainEvent(Guid OrderId, Guid BuyerId, double Latitude, double Longitude) : INotification;