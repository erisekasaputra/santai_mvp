using Core.Events;
using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class ServiceCompletedDomainEventHandler(IMediator mediator) : INotificationHandler<ServiceCompletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ServiceCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ServiceCompletedIntegrationEvent(
            notification.OrderId,
            notification.BuyerId,
            notification.MechanicId), cancellationToken);
    }
}
