using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class ItemPriceSetIntegrationEventConsumer : IConsumer<ItemPriceSetIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ItemPriceSetIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
