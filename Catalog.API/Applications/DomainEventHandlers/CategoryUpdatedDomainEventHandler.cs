using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class CategoryUpdatedDomainEventHandler(IMediator mediator) : INotificationHandler<CategoryUpdatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(CategoryUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new CategoryUpdatedIntegrationEvent(notification.Category.Id, notification.Category.Name, notification.Category.ImageUrl);

        await _mediator.Publish(@event, cancellationToken);
    }
}