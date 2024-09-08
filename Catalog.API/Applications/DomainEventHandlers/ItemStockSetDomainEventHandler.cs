 
using Catalog.Domain.Events;
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemStockSetDomainEventHandler(IMediator mediator) : INotificationHandler<ItemStockSetDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemStockSetDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemStockSetIntegrationEvent(notification.Id, notification.Quantity);

        await _mediator.Publish(@event, cancellationToken);
    }
}
