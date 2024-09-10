using Core.Events;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.DeleteItemCategory; 

namespace Search.Worker.Consumers;

public class CategoryDeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<CategoryDeletedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Consume(ConsumeContext<CategoryDeletedIntegrationEvent> context)
    {
        var message = context.Message;

        var command = new DeleteItemCategoryCommand(message.Id);

        await _mediator.Send(command);
    }
}
