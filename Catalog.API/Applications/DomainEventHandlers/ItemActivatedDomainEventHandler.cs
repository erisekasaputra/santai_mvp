using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemActivatedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemActivatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ItemActivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemActivatedIntegrationEvent(notification.Id);

        await _mediator.Publish(@event, cancellationToken);
    }
}
