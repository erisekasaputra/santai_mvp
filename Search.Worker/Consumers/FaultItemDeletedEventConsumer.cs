using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.DeleteItem;

namespace Search.Worker.Consumers;

public class FaultItemDeletedEventConsumer(IMediator mediator) : IConsumer<Fault<ItemDeletedIntegrationEvent>>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<Fault<ItemDeletedIntegrationEvent>> context)
    {
        var itemEvent = context.Message.Message;
        var command = new DeleteItemCommand(itemEvent.Id);

        Console.WriteLine(context.Message.FaultId);

        await _mediator.Send(command);
    }
}
