using Core.Events.Account;
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class MechanicDocumentRejectedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicDocumentRejectedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(MechanicDocumentRejectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
