
using Core.Events;
using Identity.API.Domain.Events; 
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class RegularUserDeleteDomainEventHandler(IMediator mediator) : INotificationHandler<RegularUserDeleteDomainEvent>
{
    private readonly IMediator _mediator= mediator;

    public async Task Handle(RegularUserDeleteDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new RegularUserDeletedIntegrationEvent(notification.UserId);

        await _mediator.Publish(@event, cancellationToken);
    }
}
