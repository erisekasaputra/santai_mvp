using Core.Events;
using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class ServiceProcessedDomainEventHandler(IMediator mediator) : INotificationHandler<ServiceProcessedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ServiceProcessedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ServiceProcessedIntegrationEvent(
                notification.OrderId,
                notification.BuyerId,
                notification.MechanicId 
            ), cancellationToken);
    }
}
