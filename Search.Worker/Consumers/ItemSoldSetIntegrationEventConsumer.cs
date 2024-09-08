 
using Core.Events;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.SetItemSoldQuantity;

namespace Search.Worker.Consumers;

internal class ItemSoldSetIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemSoldSetIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemSoldSetIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new SetItemSoldQuantityCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
