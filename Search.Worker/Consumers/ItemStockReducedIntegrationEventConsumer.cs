using Core.Events.Catalog;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.ReduceItemStockQuantity;

namespace Search.Worker.Consumers;

public class ItemStockReducedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemStockReducedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemStockReducedIntegrationEvent> context)
    {
        var @event = context.Message; 

        var command = new ReduceItemStockQuantityCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
