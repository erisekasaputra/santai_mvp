using Core.Events;
using MassTransit;
using MediatR; 

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemSoldSetIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemSoldSetIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemSoldSetIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
