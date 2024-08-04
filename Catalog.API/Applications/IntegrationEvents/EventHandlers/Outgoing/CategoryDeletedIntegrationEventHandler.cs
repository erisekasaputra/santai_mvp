using Catalog.Contracts;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class CategoryDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<CategoryDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(CategoryDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
