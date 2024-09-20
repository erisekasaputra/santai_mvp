using Core.Events.Catalog;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.DeleteItemBrand;

namespace Search.Worker.Consumers;

public class BrandDeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<BrandDeletedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Consume(ConsumeContext<BrandDeletedIntegrationEvent> context)
    {
        var message = context.Message;

        var command = new DeleteItemBrandCommand(message.Id);

        await _mediator.Send(command);
    }
}
