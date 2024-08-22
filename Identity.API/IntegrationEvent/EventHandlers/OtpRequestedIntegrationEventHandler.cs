using Identity.API.Domain.Events;
using Identity.Contracts;
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
        var integrationEvent = new OtpRequestedIntegrationEvent(notification.Address, notification.Token, notification.Provider);

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(notification.Token); 

        await _publisher.Publish(integrationEvent, cancellationToken);  
    }
}
