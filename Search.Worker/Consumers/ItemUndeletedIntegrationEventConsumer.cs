 
using Core.Events;
using MassTransit; 
using MediatR;
using Search.Worker.Applications.Commands.UndeleteItem;

namespace Search.Worker.Consumers;

public class ItemUndeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemUndeletedIntegrationEvent>
{ 
    private readonly IMediator _mediator = mediator;  

    public async Task Consume(ConsumeContext<ItemUndeletedIntegrationEvent> context)
    { 
        var @event = context.Message;

        var command = new UndeleteItemCommand(@event.Id);  

        await _mediator.Send(command); 
    }
}
