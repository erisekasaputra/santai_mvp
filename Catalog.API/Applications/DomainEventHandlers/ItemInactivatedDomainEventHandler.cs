using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemInactivatedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemInactivatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemInactivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemInactivatedIntegrationEvent(notification.Id);

        await _mediator.Publish(@event, cancellationToken);
    }
}
