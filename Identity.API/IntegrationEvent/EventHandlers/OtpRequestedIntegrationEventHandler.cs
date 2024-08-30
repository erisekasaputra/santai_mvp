using Identity.API.Domain.Events;
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;

namespace Identity.API.IntegrationEvent.EventHandlers;

public class OtpRequestedIntegrationEventHandler : INotificationHandler<OtpRequestedDomainEvent>
{  
    private readonly IPublishEndpoint _publisher;

    public OtpRequestedIntegrationEventHandler(IPublishEndpoint publishEndpoint)
    {
        _publisher = publishEndpoint;
    }


    public async Task Handle(OtpRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new OtpRequestedIntegrationEvent(notification.PhoneNumber, notification.Email, notification.Token, notification.Provider); 

        await _publisher.Publish(integrationEvent, cancellationToken);  
    }
}
