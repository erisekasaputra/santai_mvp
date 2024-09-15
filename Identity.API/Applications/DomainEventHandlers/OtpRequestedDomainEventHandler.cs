using Core.Events;
using Identity.API.Domain.Events;
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class OtpRequestedDomainEventHandler : INotificationHandler<OtpRequestedDomainEvent>
{
    private readonly IMediator _mediator;

    public OtpRequestedDomainEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }


    public async Task Handle(OtpRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new OtpRequestedIntegrationEvent(notification.PhoneNumber, notification.Email, notification.Token, notification.Provider);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
