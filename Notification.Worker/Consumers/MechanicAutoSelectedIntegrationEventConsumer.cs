using Core.Events.Account;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Notification.Worker.SeedWorks;
using Notification.Worker.Enumerations;

namespace Notification.Worker.Consumers;

public class MechanicAutoSelectedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<MechanicAutoSelectedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<MechanicAutoSelectedIntegrationEvent> context)
    {
        var orderData = context.Message; 

        var connectionId = await _cacheService.GetAsync<string>(CacheKey.GetUserCacheKey(orderData.MechanicId.ToString()));

        Console.WriteLine("Mechanic seletected with id " + orderData.MechanicId);

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
            string.Empty,
            OrderStatus.MechanicSelected);
    }
}
