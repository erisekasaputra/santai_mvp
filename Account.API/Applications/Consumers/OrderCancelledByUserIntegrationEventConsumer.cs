
using Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByUserByOrderId;
using Core.Events;
using MassTransit;
using MediatR;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByUserIntegrationEventConsumer(
    IMediator mediator) : IConsumer<OrderCancelledByUserIntegrationEvent>
{
    private readonly IMediator _mediator = mediator; 
    public async Task Consume(ConsumeContext<OrderCancelledByUserIntegrationEvent> context)
    {
        var command = new CancelOrderByUserByOrderIdCommand(context.Message.OrderId);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            throw new Exception(result.Message);
        }
    }
}
