using Catalog.Contracts;
using MassTransit;
using MediatR;
using Search.Worker.Applications.Commands.UpdateItemBrand;

namespace Search.Worker.Consumers;

public class BrandUpdatedIntegrationEventConsumer(IMediator mediator) : IConsumer<BrandUpdatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Consume(ConsumeContext<BrandUpdatedIntegrationEvent> context)
    {
        var message = context.Message; 

        var command = new UpdateItemBrandCommand(message.Id, message.Name, message.ImageUrl);

        await _mediator.Send(command);
    }
}
