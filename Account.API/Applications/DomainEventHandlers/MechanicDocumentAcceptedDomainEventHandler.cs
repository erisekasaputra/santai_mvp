using Account.Domain.Events;
using Core.Events.Account;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class MechanicDocumentAcceptedDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicDocumentVerifiedDomainEvent>
{ 
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicDocumentVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new MechanicDocumentAcceptedIntegrationEvent(notification.MechanicUser.Id, notification.MechanicUser.Name);

        await _mediator.Publish(@event, cancellationToken);
    }
}
