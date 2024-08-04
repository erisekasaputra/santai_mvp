using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemStockReducedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemStockReducedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemStockReducedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemStockReducedIntegrationEvent(notification.Id, notification.Quantity);

        await _mediator.Publish(@event, cancellationToken);
    }
}
