using Core.Events.Catalog;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.UpdateItem;

namespace Search.Worker.Consumers;

internal class ItemUpdatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemUpdatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator; 
    public async Task Consume(ConsumeContext<ItemUpdatedIntegrationEvent> context)
    { 
        var @event = context.Message;

        var command = new UpdateItemCommand(
            @event.Id,
            @event.Name,
            @event.Description,
            @event.Sku,
            @event.OldPrice,
            @event.NewPrice,
            @event.Currency,
            @event.ImageUrl,
            @event.CreatedAt,
            @event.StockQuantity,
            @event.SoldQuantity,
            @event.CategoryId,
            @event.CategoryName,
            @event.CategoryImageUrl,
            @event.BrandId,
            @event.BrandName,
            @event.BrandImageUrl,
            @event.IsActive,
            @event.IsDeleted,
            @event.OwnerReviews!);
          
        await _mediator.Send(command);
    }
}
