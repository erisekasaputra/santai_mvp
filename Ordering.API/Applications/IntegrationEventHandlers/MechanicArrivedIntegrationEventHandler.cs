using Core.Events.Ordering;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicArrivedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<OrderMechanicArrivedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(OrderMechanicArrivedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
