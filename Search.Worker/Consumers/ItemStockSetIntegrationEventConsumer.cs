using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.SetItemStockQuantity; 

namespace Search.Worker.Consumers;

internal class ItemStockSetIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemStockSetIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemStockSetIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new SetItemStockQuantityCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
