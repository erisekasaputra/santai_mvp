using Core.Events.Ordering;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;  
using Notification.Worker.Enumerations;
using Amazon.SimpleNotificationService.Model;
using Core.Configurations;
using Notification.Worker.Repository;
using Microsoft.Extensions.Options;

namespace Notification.Worker.Consumers;

public class OrderCancelledByBuyerIntegrationEventConsumer(
IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;
    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {  
        var orderData = context.Message; 
        if (orderData.MechanicId is null || orderData.MechanicId == Guid.Empty)
        {
            return;
        } 

        await _activityHubContext.Clients.User(orderData.MechanicId.ToString()!).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            orderData.BuyerName ?? string.Empty,
            orderData.MechanicId.Value.ToString() ?? string.Empty,
            orderData.MechanicName ?? string.Empty,
            OrderStatus.OrderCancelledByUser.ToString(),
            string.Empty);


        var target = await _userProfileRepository.GetProfiles(orderData.MechanicId.Value);
        if (target is null || !target.Any())
        {
            return;
        }
        foreach (var profile in target)
        {
            var fcmPayload = new
            {
                notification = new 
                {
                    title = "Santai",
                    body = $"The order has been canceled by the Customer",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP"
                },
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"The order has been canceled by the Customer",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId.Value 
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                @default = "Order cancelled by Customer",
                GCM = Newtonsoft.Json.JsonConvert.SerializeObject(fcmPayload)
            });

            var request = new PublishRequest
            {
                Message = messageJson,
                MessageStructure = "json",
                TargetArn = profile.Arn
            };

            await _messageService.PublishPushNotificationAsync(request);
        }
    }
}
