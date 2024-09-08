 
using Core.Events;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.AddItemStockQuantity;

namespace Search.Worker.Consumers;

internal class ItemStockAddedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemStockAddedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemStockAddedIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new AddItemStockQuantityCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
