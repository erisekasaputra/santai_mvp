using Catalog.API.Services;
using Catalog.Domain.Events; 
using MassTransit;
using MediatR;
using Catalog.Contracts;

namespace Catalog.API.IntegrationEvents.EventHandlers.Outgoing;

public class ItemSoldSetIntegrationEventHandler(IPublishEndpoint publishEndpoint, ApplicationService service) : INotificationHandler<ItemSoldSetDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ApplicationService _service = service;
    public async Task Handle(ItemSoldSetDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ItemSoldSetIntegrationEvent(notification.Id, notification.Quantity);

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _service.Logger.LogTrace("Publishing item sold set integration event with Item Id {id} and Amount {amount} ", notification.Id, notification.Quantity);
    }
}
