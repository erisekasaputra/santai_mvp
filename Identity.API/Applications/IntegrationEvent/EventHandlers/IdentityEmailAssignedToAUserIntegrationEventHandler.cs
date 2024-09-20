using Core.Events.Identity;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class IdentityEmailAssignedToAUserIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<IdentityEmailAssignedToAUserIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(IdentityEmailAssignedToAUserIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
