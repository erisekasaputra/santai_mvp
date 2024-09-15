using Core.Events;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

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

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(integrationEvent.Token);
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();

        await _publisher.Publish(integrationEvent, cancellationToken);
    }
}
