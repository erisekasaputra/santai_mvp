using Core.Events.Ordering;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderCancelledByBuyerIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(OrderCancelledByBuyerIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
