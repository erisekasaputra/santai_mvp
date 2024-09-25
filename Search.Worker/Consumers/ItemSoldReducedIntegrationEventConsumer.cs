using Core.Events.Catalog;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.ReduceItemSoldQuantity;

namespace Search.Worker.Consumers;

public class ItemSoldReducedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemSoldReducedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemSoldReducedIntegrationEvent> context)
    {
        var @event = context.Message;
         
        var command = new ReduceItemSoldQuantityCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
