
using Core.Events.Ordering;
using MassTransit;

namespace Chat.API.Applications.Consumers;

public class OrderCreatedIntegrationEventConsumer : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    { 
    }
}
