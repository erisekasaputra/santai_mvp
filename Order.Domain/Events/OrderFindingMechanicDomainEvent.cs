using MediatR;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Domain.Events;
 
public record OrderFindingMechanicDomainEvent(Ordering Order) : INotification;