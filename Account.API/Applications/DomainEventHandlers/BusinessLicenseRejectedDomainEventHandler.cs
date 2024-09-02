using Account.Contracts;
using Account.Domain.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class BusinessLicenseRejectedDomainEventHandler(IMediator mediator) : INotificationHandler<BusinessLicenseRejectedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(BusinessLicenseRejectedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new BusinessLicenseRejectedIntegrationEvent(notification.BusinessLicenseId);

        await _mediator.Publish(@event, cancellationToken);
    }
}
