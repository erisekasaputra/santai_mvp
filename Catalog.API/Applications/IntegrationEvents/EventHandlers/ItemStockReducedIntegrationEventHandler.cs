using Core.Events.Catalog;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemStockReducedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemStockReducedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemStockReducedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
