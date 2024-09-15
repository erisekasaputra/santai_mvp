using Core.Events; 
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class IdentityPhoneNumberConfirmedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<IdentityPhoneNumberConfirmedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(IdentityPhoneNumberConfirmedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}