 
using Catalog.Domain.Events; 
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class BrandUpdatedDomainEventHandler(IMediator mediator) : INotificationHandler<BrandUpdatedDomainEvent>
{
    private readonly IMediator _mediator = mediator; 

    public async Task Handle(BrandUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new BrandUpdatedIntegrationEvent(
            notification.Brand.Id,
            notification.Brand.Name,
            notification.Brand.ImageUrl);

        await _mediator.Publish(@event, cancellationToken);
    }
}
