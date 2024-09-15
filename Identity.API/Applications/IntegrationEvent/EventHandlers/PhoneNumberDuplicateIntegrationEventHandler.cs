using Core.Events;
using Core.Models;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class PhoneNumberDuplicateIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<PhoneNumberDuplicateIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(PhoneNumberDuplicateIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
