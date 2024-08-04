using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.UpdateItemCategory;

namespace Search.Worker.Consumers;

public class CategoryUpdatedIntegrationEventConsumer(IMediator mediator) : IConsumer<CategoryUpdatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Consume(ConsumeContext<CategoryUpdatedIntegrationEvent> context)
    {
        var message = context.Message;

        var command = new UpdateItemCategoryCommand(message.Id, message.Name, message.ImageUrl);

        await _mediator.Send(command);
    }
}
