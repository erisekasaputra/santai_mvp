using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCancelledByBuyerDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicDispatchedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicDispatchedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new MechanicDispatchedIntegrationEvent(), cancellationToken);
    }
}
