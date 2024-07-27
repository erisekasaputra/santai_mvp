using Catalog.API.Services;
using Catalog.Contracts;
using Catalog.Domain.Events;
using MassTransit;
using MediatR;

namespace Catalog.API.IntegrationEvents.EventHandlers.Outgoing;

public class ItemDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint, ApplicationService service) : INotificationHandler<ItemDeletedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    private readonly ApplicationService _service = service;

    public async Task Handle(ItemDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ItemDeletedIntegrationEvent(notification.Id);

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _service.Logger.LogInformation("Item deleted : {item}", integrationEvent);
    }
}
