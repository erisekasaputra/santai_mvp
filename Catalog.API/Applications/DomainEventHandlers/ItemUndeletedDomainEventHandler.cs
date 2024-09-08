 
using Catalog.Domain.Events;
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemUndeletedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemUndeletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemUndeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemUndeletedIntegrationEvent(notification.Id);

        await _mediator.Publish(@event, cancellationToken);
    }
}
