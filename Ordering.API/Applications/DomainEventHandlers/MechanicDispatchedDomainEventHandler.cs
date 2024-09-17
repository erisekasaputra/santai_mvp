using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class MechanicDispatchedDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicDispatchedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicDispatchedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new MechanicDispatchedIntegrationEvent(
            notification.OrderId,
            notification.BuyerId,
            notification.MechanicId), cancellationToken);
    }
}

