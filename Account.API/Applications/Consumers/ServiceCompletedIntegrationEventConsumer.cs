using Account.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;

namespace Account.API.Applications.Consumers;

public class ServiceCompletedIntegrationEventConsumer(IMechanicCache mechanicCache) : IConsumer<ServiceCompletedIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    public async Task Consume(ConsumeContext<ServiceCompletedIntegrationEvent> context)
    {
        (var isSuccess, _) = await _mechanicCache.CompleteOrder(
            context.Message.OrderId.ToString(), 
            context.Message.MechanicId.ToString());

        if (isSuccess)
        {
            throw new Exception($"Failed when completing the order {context.Message.OrderId}");
        }
    }
}
