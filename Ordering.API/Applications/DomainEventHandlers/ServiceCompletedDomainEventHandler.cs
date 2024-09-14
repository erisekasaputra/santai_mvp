using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class ServiceCompletedDomainEventHandler(IMediator mediator) : INotificationHandler<ServiceCompletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ServiceCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ServiceCompletedIntegrationEvent(
            notification.Order.Id,
            notification.Order.Buyer.BuyerId,
            notification.Order.Mechanic!.MechanicId), cancellationToken);
    }
}
