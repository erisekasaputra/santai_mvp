 
using Core.Events;
using MassTransit;
using MediatR; 
using Search.Worker.Applications.Commands.CreateItem; 

namespace Search.Worker.Consumers;

internal class ItemCreatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemCreatedIntegrationEvent>
{  
    private readonly IMediator _mediator = mediator; 
    public async Task Consume(ConsumeContext<ItemCreatedIntegrationEvent> context)
    { 
        var @event = context.Message;

        var command = new CreateItemCommand(
            @event.Id,
            @event.Name,
            @event.Description,
            @event.Sku,
            @event.Price,
            @event.Currency,
            @event.ImageUrl, 
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
