using Core.Events.Account;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicAutoSelectedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicAutoSelectedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(MechanicAutoSelectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
