using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class ServiceIncompletedDomainEventHandler(IMediator mediator) : INotificationHandler<ServiceIncompletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ServiceIncompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ServiceIncompletedIntegrationEvent(
                notification.OrderId,
                notification.BuyerId,
                notification.MechanicId
            ), cancellationToken);
    }
}
