using Core.Events; 
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class OtpRequestedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<OtpRequestedIntegrationEvent>
{
    private readonly IPublishEndpoint _publisher = publishEndpoint; 

    public async Task Handle(OtpRequestedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publisher.Publish(notification, cancellationToken);
    }
}
 