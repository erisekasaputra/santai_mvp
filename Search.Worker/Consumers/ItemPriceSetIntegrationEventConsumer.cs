 
using Core.Events;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.SetItemPrice; 

namespace Search.Worker.Consumers;

internal class ItemPriceSetIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemPriceSetIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemPriceSetIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new SetItemPriceCommand(@event.Id, @event.Amount);

        await _mediator.Send(command);
    }
}
