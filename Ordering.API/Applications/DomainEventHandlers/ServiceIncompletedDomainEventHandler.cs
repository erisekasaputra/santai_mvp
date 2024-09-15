using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class ServiceIncompletedDomainEventHandler(IMediator mediator) : INotificationHandler<ServiceIncompletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ServiceIncompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new ServiceIncompletedIntegrationEvent(
                notification.Order.Id,
                notification.Order.Buyer.BuyerId,
                notification.Order.Mechanic!.MechanicId
            ), cancellationToken);
    }
}
