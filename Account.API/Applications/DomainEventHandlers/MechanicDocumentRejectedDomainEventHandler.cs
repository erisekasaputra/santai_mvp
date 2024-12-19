using Account.Domain.Events;
using Core.Events.Account;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class MechanicDocumentRejectedDomainEventHandler(IMediator mediator) : INotificationHandler<MechanicDocumentRejectedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicDocumentRejectedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new MechanicDocumentRejectedIntegrationEvent(notification.MechanicUser.Id, notification.MechanicUser.Name);

        await _mediator.Publish(@event, cancellationToken);
    }
}
