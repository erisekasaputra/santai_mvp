using Core.Events;
using Identity.API.Domain.Events; 
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class EmailAssignedToAUserDomainEventHandler(IMediator mediator) : INotificationHandler<EmailAssignedToAUserDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(EmailAssignedToAUserDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new IdentityEmailAssignedToAUserIntegrationEvent(
            notification.Sub, notification.Email, notification.UserType);

        await _mediator.Publish(@event, cancellationToken);
    }
}
