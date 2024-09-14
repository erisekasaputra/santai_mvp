using Core.Enumerations;
using Core.Events;
using Core.ValueObjects;
using MediatR;
using Ordering.Domain.Aggregates.BuyerAggregate;
using Ordering.Domain.Aggregates.MechanicAggregate;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Enumerations;
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCreatedDomainEventHandler(IMediator mediator) : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
    //public Address Address { get; private set; }
    //public Buyer Buyer { get; private set; }
    //public Mechanic? Mechanic { get; private set; }
    //public OrderStatus Status { get; private set; }
    //public ICollection<LineItem> LineItems { get; private set; }
    //public ICollection<Fleet> Fleets { get; private set; }
    //public int TotalCanceledByMechanic { get; private set; }
    //public DateTime CreatedAtUtc { get; private init; }
    //public bool IsScheduled { get; private set; }
    //public DateTime? ScheduledOnUtc { get; private set; }
    //public Payment? Payment { get; private set; }
    //public string? PaymentUrl { get; private set; }
    //public DateTime PaymentExpiration { get; private set; }
    //public Currency Currency { get; private set; }
    //public decimal OrderAmount { get; private set; }
    //public Discount? Discount { get; private set; }
    //public Money GrandTotal { get; private set; }
    await _mediator.Send(new OrderCreatedIntegrationEvent(
            notification.Order.Id,
            notification.Order.Buyer.BuyerId,
            notification.Order.Buyer.Name,
            notification.Order.Address.AddressLine,
            notification.Order.Address.Latitude,
            notification.Order.Address.Longitude,
            notification.Order.), cancellationToken);
    }
}
