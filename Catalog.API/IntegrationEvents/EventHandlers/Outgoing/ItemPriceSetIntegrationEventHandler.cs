using Catalog.API.Services;
using Catalog.Contracts;
using Catalog.Domain.Events;
using MassTransit;
using MediatR;

namespace Catalog.API.IntegrationEvents.EventHandlers.Outgoing;

public class ItemPriceSetIntegrationEventHandler(IPublishEndpoint publishEndpoint, ApplicationService service) : INotificationHandler<ItemPriceSetDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ApplicationService _service = service;
    public async Task Handle(ItemPriceSetDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ItemPriceSetIntegrationEvent(notification.Id, notification.Amount);

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _service.Logger.LogTrace("Publishing item price set integration event with Item Id {id} and Amount {amount} ", notification.Id, notification.Amount);
    }
}
