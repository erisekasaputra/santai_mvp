using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.InactivateItem;

namespace Search.Worker.Consumers;

public class ItemInactivatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ItemInactivatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
        
    public async Task Consume(ConsumeContext<ItemInactivatedIntegrationEvent> context)
    {  
        var @event = context.Message;

        var command = new InactivateItemCommand(@event.Id);

        await _mediator.Send(command); 
    }
}
