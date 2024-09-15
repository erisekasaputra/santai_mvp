using Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByMechanicByIdByOrderId;
using Azure;
using Core.Events;
using Core.Results;
using MassTransit;
using MediatR;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer(
    IMediator mediator) : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        var command = new CancelOrderByMechanicByIdByOrderIdCommand(context.Message.MechanicId, context.Message.OrderId);
          
        var result = await _mediator.Send(command);

        if (!result.IsSuccess) 
        {
            if (result.ResponseStatus is not ResponseStatus.NotFound and ResponseStatus.BadRequest)
            {
                throw new Exception(result.Message);
            }
        }
    }
}
