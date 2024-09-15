using Core.Events;
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class AccountMechanicOrderAcceptedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint): INotificationHandler<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(AccountMechanicOrderAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
