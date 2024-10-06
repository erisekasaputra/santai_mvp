using Amazon.SimpleNotificationService.Model;
using Core.Configurations;
using Core.Events.Account;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Notification.Worker.Enumerations; 
using Notification.Worker.Repository;
using Notification.Worker.Services;
using Notification.Worker.Services.Interfaces;

namespace Notification.Worker.Consumers;
public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
    {
        var orderData = context.Message; 
        await _activityHubContext.Clients.User(orderData.BuyerId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            orderData.MechanicId.ToString(),
            orderData.MechanicName,
            OrderStatus.MechanicAssigned.ToString(),
            string.Empty);



        var target = await _userProfileRepository.GetProfiles(orderData.BuyerId); 
        if (target is null || !target.Any())
        { 
            return;
        } 
        foreach(var profile in target)
        {
            var fcmPayload = new
            {
                notification = new 
                {
                    title = "Santai",
                    body = $"Successfully assigned a mechanic. Mechanic {orderData.MechanicName} has been assigned and will be heading to your location shortly",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP"
                },
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"Successfully assigned a mechanic. Mechanic {orderData.MechanicName} has been assigned and will be heading to your location shortly",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId,
                    mechanicName = orderData.MechanicName
                }
            }; 

            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            { 
                @default = $"Mechanic {orderData.MechanicName} has been assigned",
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
