using Core.Events;
using Core.Models;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class PhoneNumberDuplicateIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<PhoneNumberDuplicateDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(PhoneNumberDuplicateDomainEvent notification, CancellationToken cancellationToken)
    {
        var duplicateUsers = new List<DuplicateUser>();

        foreach(var duplicateUser in notification.Users)
        {
            duplicateUsers.Add(new DuplicateUser(Guid.Parse(duplicateUser.Id), duplicateUser.PhoneNumber, duplicateUser.UserType));
        }

        var @event = new PhoneNumberDuplicateIntegrationEvent(duplicateUsers); 
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
