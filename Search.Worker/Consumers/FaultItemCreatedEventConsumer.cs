using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.CreateItem;

namespace Search.Worker.Consumers;

public class FaultItemCreatedEventConsumer(IMediator mediator) : IConsumer<Fault<ItemCreatedIntegrationEvent>>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<Fault<ItemCreatedIntegrationEvent>> context)
    {
        var itemEvent = context.Message.Message;
        var command = new CreateItemCommand(itemEvent.Id, itemEvent.Name, itemEvent.Description, itemEvent.Price, itemEvent.ImageUrl, itemEvent.CreatedAt, itemEvent.StockQuantity, itemEvent.SoldQuantity, itemEvent.CategoryId, itemEvent.CategoryName, itemEvent.BrandId, itemEvent.BrandName);

        Console.WriteLine(context.Message.FaultId);

        await _mediator.Send(command);
    }
}
