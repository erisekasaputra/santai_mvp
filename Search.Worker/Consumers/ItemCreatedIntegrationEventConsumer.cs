using Catalog.Contracts;
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

        var command = new CreateItemCommand(@event.Id, @event.Name, @event.Description, @event.Price, @event.ImageUrl, @event.CreatedAt, @event.StockQuantity, @event.SoldQuantity, @event.CategoryId, @event.CategoryName, @event.BrandId, @event.BrandName, @event.OwnerReviews!); 

        await _mediator.Send(command);
    }
}
