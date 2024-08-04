using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemStockAddedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemStockAddedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemStockAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemStockAddedIntegrationEvent(notification.Id, notification.Quantity);

        await _mediator.Publish(@event, cancellationToken);
    }
}
