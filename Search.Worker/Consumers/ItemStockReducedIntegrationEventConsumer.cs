using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.ReduceStock;

namespace Search.Worker.Consumers;

internal class ItemStockReducedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemStockReducedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemStockReducedIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new ReduceStockCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
