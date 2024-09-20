using Core.Events.Catalog;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemCreatedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemCreatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
