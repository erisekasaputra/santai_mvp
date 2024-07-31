using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.SetSold;

namespace Search.Worker.Consumers;

internal class ItemSoldSetIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemSoldSetIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemSoldSetIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new SetSoldCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
