 
using Catalog.Domain.Events;
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemDeletedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemDeletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemDeletedIntegrationEvent(notification.Id);

        await _mediator.Publish(@event, cancellationToken);
    }
}
