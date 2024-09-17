using Account.API.Applications.Services.Interfaces; 
using Core.Events;
using MassTransit;

namespace Account.API.Applications.Consumers;

public class ServiceIncompletedIntegrationEventConsumer(IMechanicCache mechanicCache) : IConsumer<ServiceIncompletedIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    public async Task Consume(ConsumeContext<ServiceIncompletedIntegrationEvent> context)
    {
        var result = await _mechanicCache.CompleteOrder(
         context.Message.OrderId.ToString(),
         context.Message.MechanicId.ToString());

        if (result)
        {
            throw new Exception($"Failed when completing the order {context.Message.OrderId}");
        }
    }
}
