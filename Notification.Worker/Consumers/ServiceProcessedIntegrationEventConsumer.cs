using Core.Events.Ordering;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Notification.Worker.Enumerations; 
using Core.Configurations;
using Notification.Worker.Repository;
using Microsoft.Extensions.Options;
using Amazon.SimpleNotificationService.Model;

namespace Notification.Worker.Consumers;

public class ServiceProcessedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration) : IConsumer<ServiceProcessedIntegrationEvent>
{
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;
    public async Task Consume(ConsumeContext<ServiceProcessedIntegrationEvent> context)
    {
        var orderData = context.Message; 
        await _activityHubContext.Clients.User(orderData.BuyerId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            orderData.MechanicId.ToString(),
            string.Empty,
            OrderStatus.ServiceInProgress.ToString(),
            string.Empty);




        var target = await _userProfileRepository.GetProfiles(orderData.BuyerId);
        if (target is null || !target.Any())
        {
            return;
        }
        foreach (var profile in target)
        {
            var fcmPayload = new
            {
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"Your vehicle service is starting",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                @default = "Service is starting",
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
