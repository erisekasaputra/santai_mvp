using Account.Contracts;
using Account.Domain.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class BusinessLicenseAcceptedDomainEventHandler(IMediator mediator) : INotificationHandler<BusinessLicenseAcceptedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(BusinessLicenseAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new BusinessLicenseAcceptedIntegrationEvent(notification.BusinessLicenseId);

        await _mediator.Publish(@event, cancellationToken);
    }
}
