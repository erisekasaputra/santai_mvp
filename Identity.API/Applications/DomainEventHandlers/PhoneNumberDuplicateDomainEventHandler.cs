using Core.Events;
using Core.Models;
using Identity.API.Domain.Events; 
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class PhoneNumberDuplicateDomainEventHandler(IMediator mediator) : INotificationHandler<PhoneNumberDuplicateDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(PhoneNumberDuplicateDomainEvent notification, CancellationToken cancellationToken)
    { 
        var @event = new PhoneNumberDuplicateIntegrationEvent(notification.Users);
        await _mediator.Publish(@event, cancellationToken);
    }
}
