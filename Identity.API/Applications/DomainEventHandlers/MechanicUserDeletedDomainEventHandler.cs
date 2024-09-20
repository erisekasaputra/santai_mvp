
using Core.Events;
using Core.Events.Account;
using Identity.API.Domain.Events; 
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class MechanicUserDeletedDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicUserDeletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicUserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new MechanicUserDeletedIntegrationEvent(notification.UserId);

        await _mediator.Publish(@event, cancellationToken);
    }
}
