using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.SetPrice;

namespace Search.Worker.Consumers;

internal class ItemPriceSetIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemPriceSetIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemPriceSetIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new SetPriceCommand(@event.Id, @event.Amount);

        await _mediator.Send(command);
    }
}
