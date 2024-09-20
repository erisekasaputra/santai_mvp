 
using Catalog.Domain.Events;
using Core.Events;
using Core.Events.Catalog;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemSoldSetDomainEventHandler(IMediator mediator) : INotificationHandler<ItemSoldSetDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemSoldSetDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemSoldSetIntegrationEvent(notification.Id, notification.Quantity);

        await _mediator.Publish(@event, cancellationToken);
    }
}
