using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.DeleteItem;

namespace Search.Worker.Consumers;

public class ItemDeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemDeletedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator; 
    public async Task Consume(ConsumeContext<ItemDeletedIntegrationEvent> context)
    { 
        var itemEvent = context.Message;
        var command = new DeleteItemCommand(itemEvent.Id);

        Console.WriteLine(itemEvent.Id);

        await _mediator.Send(command);
    }
}
