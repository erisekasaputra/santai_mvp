 
using Core.Events;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.DeleteItem;

namespace Search.Worker.Consumers;

internal class ItemDeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemDeletedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator; 
    public async Task Consume(ConsumeContext<ItemDeletedIntegrationEvent> context)
    { 
        var @event = context.Message; 

        var command = new DeleteItemCommand(@event.Id);  

        await _mediator.Send(command);
    }
}
