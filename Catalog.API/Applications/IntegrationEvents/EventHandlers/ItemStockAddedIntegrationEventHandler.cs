
using Core.Events;
using MassTransit;
using MediatR; 

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemStockAddedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemStockAddedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(ItemStockAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
