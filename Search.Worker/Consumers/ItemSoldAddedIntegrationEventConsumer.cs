using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.AddSold;

namespace Search.Worker.Consumers;

internal class ItemSoldAddedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemSoldAddedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemSoldAddedIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new AddSoldCommand(@event.Id, @event.Quantity);

        await _mediator.Send(command);
    }
}
