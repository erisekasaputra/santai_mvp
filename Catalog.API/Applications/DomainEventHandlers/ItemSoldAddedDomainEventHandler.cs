using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemSoldAddedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemSoldAddedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemSoldAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemSoldAddedIntegrationEvent(notification.Id, notification.Quantity);

        await _mediator.Publish(@event, cancellationToken);
    }
} 
