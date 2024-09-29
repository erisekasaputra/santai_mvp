using Core.Events.Account;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services; 
using Notification.Worker.Enumerations;
using Amazon.SimpleNotificationService.Model;
using Notification.Worker.Repository;
using Core.Configurations;
using Microsoft.Extensions.Options;
namespace Notification.Worker.Consumers;
public class MechanicAutoSelectedIntegrationEventConsumer(
IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration) : IConsumer<MechanicAutoSelectedIntegrationEvent>
{
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;
    public async Task Consume(ConsumeContext<MechanicAutoSelectedIntegrationEvent> context)
    {
        var orderData = context.Message;   
        await _activityHubContext.Clients.User(orderData.MechanicId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            orderData.MechanicId.ToString(),
            string.Empty,
            OrderStatus.MechanicSelected.ToString(),
            string.Empty);
         
        

        var target = await _userProfileRepository.GetProfiles(orderData.BuyerId);
        if (target is null || !target.Any())
        {
            return;
        }
        foreach (var profile in target)
        {
            var confirmSeconds = orderData.ConfirmDeadlineInSeconds; 
            int minutes = confirmSeconds / 60; 
            int seconds = confirmSeconds % 60; 

            var fcmPayload = new
            {
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"You have received a new order, the confirmation time is {minutes} minutes and {seconds} seconds",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP",
                    actions = new[]
                    {
                        new
                        {
                            title = "ACCEPT",
                            action = "accept_action",
                            icon = "ic_accept"
                        },
                        new
                        {
                            title = "DECLINE",
                            action = "decline_action",
                            icon = "ic_decline"
                        }
                    },
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId 
                }
            };

            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                @default = "You have received a new Order",
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
