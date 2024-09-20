using Core.Events.Catalog;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemUndeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemUndeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemUndeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
