using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;

namespace Identity.API.IntegrationEvent.EventHandlers;

public class PhoneNumberDuplicateIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<PhoneNumberDuplicateIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(PhoneNumberDuplicateIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
