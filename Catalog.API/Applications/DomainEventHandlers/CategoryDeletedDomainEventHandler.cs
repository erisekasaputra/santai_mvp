 
using Catalog.Domain.Events;
using Catalog.Domain.SeedWork;
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class CategoryDeletedDomainEventHandler(IMediator mediator, IUnitOfWork unitOfWork) : INotificationHandler<CategoryDeletedDomainEvent>
{
    private readonly IMediator _mediator = mediator; 

    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(CategoryDeletedDomainEvent notification, CancellationToken cancellationToken)
    { 
        await _unitOfWork.Items.MarkCategoryIdToNullByDeletingCategoryByIdAsync(notification.Id);

        var @event = new CategoryDeletedIntegrationEvent(notification.Id);

        await _mediator.Publish(@event, cancellationToken);
    }
}