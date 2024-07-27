using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.UpdateItem;

namespace Search.Worker.Consumers;

public class ItemUpdatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemUpdatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator; 
    public async Task Consume(ConsumeContext<ItemUpdatedIntegrationEvent> context)
    { 
        var itemEvent = context.Message;
        var command = new UpdateItemCommand(itemEvent.Id, itemEvent.Name, itemEvent.Description, itemEvent.Price, itemEvent.ImageUrl, itemEvent.CreatedAt, itemEvent.StockQuantity, itemEvent.SoldQuantity, itemEvent.CategoryId, itemEvent.CategoryName, itemEvent.BrandId, itemEvent.BrandName);

        Console.WriteLine(itemEvent.Id);

        await _mediator.Send(command);
    }
}
