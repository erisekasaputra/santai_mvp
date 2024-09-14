using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class MechanicArrivedDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicArrivedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicArrivedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(, cancellationToken);
    }
}
