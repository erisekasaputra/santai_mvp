using Account.API.Applications.Commands.OrderTaskCommand.CreateOrderTask;
using Core.Events;
using MassTransit;
using MediatR;

namespace Account.API.Applications.Consumers;

public class OrderFindingMechanicIntegrationEventConsumer(
    IMediator mediator) : IConsumer<OrderFindingMechanicIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<OrderFindingMechanicIntegrationEvent> context)
    {
        var order = context.Message;
        await _mediator.Send(new CreateOrderTaskCommand(order.OrderId, order.Latitude, order.Longitude));
    }
}
