using Core.Events.Catalog;
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

        var command = new SetItemPriceCommand(
            @event.Id, 
            @event.OldAmount, 
            @event.NewAmount, 
            @event.Currency);

        await _mediator.Send(command);
    }
}
