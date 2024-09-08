using Account.Domain.Events;
using Core.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class StaffDeletedDomainEventHandler(
    IMediator mediator) : INotificationHandler<StaffDeletedDomainEvent>
{ 
    private readonly IMediator _mediator = mediator;
    public async Task Handle(StaffDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var staffId = notification.Staff.Id;
        await _mediator.Publish(new StaffUserDeletedIntegrationEvent(staffId), cancellationToken);
    }
}
