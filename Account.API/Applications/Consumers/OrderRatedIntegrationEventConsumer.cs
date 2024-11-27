using Account.API.Applications.Commands.MechanicUserCommand.SetRatingByUserId;
using Core.Events.Ordering;
using Core.Results;
using MassTransit;
using MediatR;

namespace Account.API.Applications.Consumers;

public class OrderRatedIntegrationEventConsumer(IMediator mediator) : IConsumer<OrderRatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Consume(ConsumeContext<OrderRatedIntegrationEvent> context)
    {
        var command = new SetRatingByUserIdCommand(context.Message.MechanicId, context.Message.Rating);

        var result = await _mediator.Send(command);

        if (result.ResponseStatus is ResponseStatus.NotFound or ResponseStatus.BadRequest)
        {
            return;
        }

        if (!result.IsSuccess) 
        {
            throw new Exception(result.Message);
        }
    }
}
