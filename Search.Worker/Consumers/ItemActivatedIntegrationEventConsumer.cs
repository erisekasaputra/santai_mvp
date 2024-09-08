 
using Core.Events;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.ActivateItem;

namespace Search.Worker.Consumers;

public class ItemActivatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemActivatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ItemActivatedIntegrationEvent> context)
    { 
        var @event = context.Message;

        var command = new ActivateItemCommand(@event.Id);

        await _mediator.Send(command);
    }
}
