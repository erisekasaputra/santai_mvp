using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Core.Events; 
using MassTransit; 

namespace Account.API.Applications.Consumers;

public class OrderFindingMechanicIntegrationEventConsumer(
    IMechanicCache cache) : IConsumer<OrderFindingMechanicIntegrationEvent>
{
    private readonly IMechanicCache _cache = cache;
    public async Task Consume(ConsumeContext<OrderFindingMechanicIntegrationEvent> context)
    {
        await _cache.CreateOrderToQueueAndHash(
                   new OrderTask(
                       context.Message.BuyerId.ToString(),
                       context.Message.OrderId.ToString(),
                       string.Empty,
                       context.Message.Latitude,
                       context.Message.Longitude,
                       OrderTaskStatus.WaitingMechanic));
    }
}
