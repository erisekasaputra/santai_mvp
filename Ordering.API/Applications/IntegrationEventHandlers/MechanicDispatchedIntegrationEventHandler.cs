using Core.Events.Account;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicDispatchedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicDispatchedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(MechanicDispatchedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
