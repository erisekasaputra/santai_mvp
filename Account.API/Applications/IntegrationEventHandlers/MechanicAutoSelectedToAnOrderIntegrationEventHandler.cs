using Core.Events.Account;
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class MechanicAutoSelectedToAnOrderIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicAutoSelectedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(MechanicAutoSelectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
