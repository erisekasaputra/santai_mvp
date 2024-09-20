using Core.Events.Ordering;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderCreatedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<OrderCreatedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
