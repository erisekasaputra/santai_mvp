using Account.Domain.Events;
using Identity.Contracts.IntegrationEvent;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class RegularUserDeletedDomainEventHandler(
    IMediator mediator) : INotificationHandler<RegularUserDeletedDomainEvent>
{ 
    private readonly IMediator _mediator = mediator;
    public async Task Handle(RegularUserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new RegularUserDeletedIntegrationEvent(notification.Id), cancellationToken);
    }
}
