using Core.Events.Catalog;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemInactivatedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemInactivatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemInactivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
