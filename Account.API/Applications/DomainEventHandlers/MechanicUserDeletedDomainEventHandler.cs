using Account.API.Services;
using Account.Domain.Events;
using Identity.Contracts.IntegrationEvent;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class MechanicUserDeletedDomainEventHandler(
    IMediator mediator) : INotificationHandler<MechanicUserDeletedDomainEvent>
{ 
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicUserDeletedDomainEvent notification, CancellationToken cancellationToken)
    { 
        await _mediator.Publish(new MechanicUserDeletedIntegrationEvent(notification.Id), cancellationToken);
    } 
}
