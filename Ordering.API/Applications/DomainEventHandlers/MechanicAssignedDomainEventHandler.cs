using Core.Events;
using Core.Events.Account;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class MechanicAssignedDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicAssignedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new MechanicAssignedIntegrationEvent(
            notification.OrderId,
            notification.BuyerId,
            notification.MechanicId,
            notification.MechanicName), cancellationToken);
    }
}
