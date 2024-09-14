using Core.Events;
using Core.Events.Models;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCreatedDomainEventHandler(IMediator mediator) : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {   
        var fleets = notification.Order.Fleets.Select(x => new OrderFleetEventModel(
            x.Id,
            x.OrderId,
            x.FleetId,
            x.Brand,
            x.Model,
            x.RegistrationNumber,
            x.ImageUrl));

        var lineItems = notification.Order.LineItems.Select(x => new OrderLineItemEventModel(
            x.Id,
            x.LineItemId,
            x.Name,
            x.Sku,
            x.UnitPrice,
            x.SubTotal.Currency,
            x.Quantity,
            x.Tax?.TaxAmount.Amount ?? 0,
            x.SubTotal.Amount));

        await _mediator.Send(new OrderCreatedIntegrationEvent(
            notification.Order.Id,
            notification.Order.Buyer.BuyerId,
            notification.Order.Buyer.Name,
            notification.Order.Address.AddressLine,
            notification.Order.Address.Latitude,
            notification.Order.Address.Longitude,
            lineItems,
            fleets,
            notification.Order.IsScheduled,
            notification.Order.ScheduledOnUtc,
            notification.Order.OrderAmount,
            notification.Order.Currency,
            notification.Order.PaymentUrl,
            notification.Order.PaymentExpiration,
            notification.Order.Discount?.DiscountAmount.Amount ?? 0,
            notification.Order.GrandTotal.Amount
            ), cancellationToken);
    }
}
