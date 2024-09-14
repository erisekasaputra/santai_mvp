using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class ServiceProcessedDomainEventHandler(IMediator mediator) : INotificationHandler<ServiceProcessedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ServiceProcessedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ServiceProcessedIntegrationEvent(
                notification.Order.Id,
                notification.Order.Buyer.BuyerId,
                notification.Order.Mechanic!.MechanicId 
            ), cancellationToken);
    }
}
