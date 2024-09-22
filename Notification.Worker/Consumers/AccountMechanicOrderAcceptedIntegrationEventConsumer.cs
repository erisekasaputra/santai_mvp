using Core.Events.Account;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Enumerations;
using Notification.Worker.SeedWorks;
using Notification.Worker.Services;
using Notification.Worker.Services.Interfaces;

namespace Notification.Worker.Consumers;
public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
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
            orderData.MechanicId.ToString(),
            orderData.MechanicName,
            OrderStatus.MechanicAssigned.ToString(),
            string.Empty); 
    }
}
