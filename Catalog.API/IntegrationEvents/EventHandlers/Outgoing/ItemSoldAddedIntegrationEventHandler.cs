using Catalog.API.Services;
using Catalog.Domain.Events; 
using MassTransit;
using MediatR;
using Catalog.Contracts;

namespace Catalog.API.IntegrationEvents.EventHandlers.Outgoing;

public class ItemSoldAddedIntegrationEventHandler(IPublishEndpoint publishEndpoint, ApplicationService service) : INotificationHandler<ItemSoldAddedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ApplicationService _service = service;
    public async Task Handle(ItemSoldAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ItemSoldAddedIntegrationEvent(notification.Id, notification.Quantity);

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _service.Logger.LogTrace("Publishing item sold added integration event with Item Id {id} and Amount {amount} ", notification.Id, notification.Quantity);
    }
}
