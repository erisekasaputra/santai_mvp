using Core.Events.Ordering;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Notification.Worker.SeedWorks;
using Notification.Worker.Enumerations;

namespace Notification.Worker.Consumers;

public class ServiceCompletedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<ServiceCompletedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<ServiceCompletedIntegrationEvent> context)
    {
        var orderData = context.Message;
        var connectionId = await _cacheService.GetAsync<string>(CacheKey.GetUserCacheKey(orderData.BuyerId.ToString()));

        if (connectionId is null || string.IsNullOrEmpty(connectionId))
        {
            // send notification via sns
            return;
        }

        await _activityHubContext.Clients.User(connectionId).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            string.Empty,
            string.Empty,
            OrderStatus.ServiceCompleted);
    }
}
