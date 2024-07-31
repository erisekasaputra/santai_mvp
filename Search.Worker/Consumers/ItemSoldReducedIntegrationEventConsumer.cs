using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.ReduceSold;

namespace Search.Worker.Consumers;

internal class ItemSoldReducedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemSoldReducedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemSoldReducedIntegrationEvent> context)
    {
        var @event = context.Message;

        var command = new ReduceSoldCommand(@event.Id, @event.Quantiy);

        await _mediator.Send(command);
    }
}
