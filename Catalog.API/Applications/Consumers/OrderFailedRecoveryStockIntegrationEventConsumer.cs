using Catalog.API.Applications.Commands.Items.AddItemStockQuantity;
using Core.Events;
using Core.Results;
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.Consumers;

public class OrderFailedRecoveryStockIntegrationEventConsumer(
    IMediator mediator,
    ILogger<OrderFailedRecoveryStockIntegrationEventConsumer> logger): IConsumer<OrderFailedRecoveryStockIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<OrderFailedRecoveryStockIntegrationEventConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<OrderFailedRecoveryStockIntegrationEvent> context)
    {  
        var command = new AddItemStockQuantityCommand(
            context.Message.Items.Select(x => new AddItemStockQuantityRequest(x.Id, x.Quantity)));

        var result = await _mediator.Send(command);  

        if (result.ResponseStatus is not ResponseStatus.UnprocessableEntity and ResponseStatus.NotFound)
        {
            _logger.LogError(result.Message);
            throw new Exception(result.Message);
        }
    }
}
