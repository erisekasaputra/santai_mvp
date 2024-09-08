 
using Catalog.Domain.Events;
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemSoldReducedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemSoldReducedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemSoldReducedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemSoldReducedIntegrationEvent(notification.Id, notification.Quantity);

        await _mediator.Publish(@event, cancellationToken);
    }
}
