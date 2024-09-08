using Core.Events; 
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class RegularUserCreatedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<RegularUserCreatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(RegularUserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
