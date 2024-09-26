using Core.Events.Ordering;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Notification.Worker.Enumerations;
using Notification.Worker.SeedWorks;

namespace Notification.Worker.Consumers;

public class ServiceIncompletedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<ServiceIncompletedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<ServiceIncompletedIntegrationEvent> context)
    {
        var orderData = context.Message; 
        await _activityHubContext.Clients.User(connectionId).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            string.Empty,
            string.Empty,
            OrderStatus.ServiceIncompleted.ToString(),
            string.Empty);
    }
}
