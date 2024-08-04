using Catalog.Contracts;
using Catalog.Domain.Events;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class BrandDeletedDomainEventHandler(IMediator mediator, IUnitOfWork unitOfWork) : INotificationHandler<BrandDeletedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(BrandDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _unitOfWork.Items.MarkBrandIdToNullByDeletingBrandByIdAsync(notification.Id);

        var @event = new BrandDeletedIntegrationEvent(notification.Id);

        await _mediator.Publish(@event, cancellationToken);
    }
}
