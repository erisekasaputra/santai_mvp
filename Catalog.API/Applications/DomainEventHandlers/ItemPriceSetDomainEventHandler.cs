using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemPriceSetDomainEventHandler(IMediator mediator) : INotificationHandler<ItemPriceSetDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemPriceSetDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ItemPriceSetIntegrationEvent(notification.Id, notification.Amount);

        await _mediator.Publish(@event, cancellationToken);
    }
}
