using Core.Events.Ordering;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderCancelledByMechanicIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(OrderCancelledByMechanicIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
