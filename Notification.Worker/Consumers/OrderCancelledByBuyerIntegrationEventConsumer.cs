using Core.Events.Ordering;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Notification.Worker.SeedWorks;
using Notification.Worker.Enumerations;

namespace Notification.Worker.Consumers;

public class OrderCancelledByBuyerIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {  
        var orderData = context.Message; 
        if (orderData.MechanicId is null || orderData.MechanicId == Guid.Empty)
        {
            return;
        }

        var connectionId = await _cacheService.GetAsync<string>(CacheKey.GetUserCacheKey(orderData.MechanicId.ToString()!));

        if (connectionId is null || string.IsNullOrEmpty(connectionId))
        {
            // send notification via sns
            return;
        }


        await _activityHubContext.Clients.User(connectionId).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            orderData.BuyerName,
            orderData.MechanicId.ToString() ?? string.Empty,
            orderData.MechanicName,
            OrderStatus.OrderCancelledByUser.ToString(),
            string.Empty);
    }
}
