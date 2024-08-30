using MassTransit;
using MediatR; 
using Identity.Contracts.IntegrationEvent;

namespace Account.API.Applications.IntegrationEventHandlers;


public class BusinessUserCreatedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<BusinessUserCreatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(BusinessUserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}