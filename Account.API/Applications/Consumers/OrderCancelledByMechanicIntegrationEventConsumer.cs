using Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByMechanicByIdByOrderId;
using Core.Events;
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
        await _mediator.Send(command);
    }
}
