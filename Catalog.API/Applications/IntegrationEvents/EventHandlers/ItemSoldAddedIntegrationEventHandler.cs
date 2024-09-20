using Core.Events.Catalog;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemSoldAddedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemSoldAddedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemSoldAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
