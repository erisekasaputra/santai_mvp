using Core.Events.Account;
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class MechanicDocumentAcceptedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicDocumentAcceptedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(MechanicDocumentAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
