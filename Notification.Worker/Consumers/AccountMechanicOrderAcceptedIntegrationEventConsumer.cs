using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core.Configurations;
using Core.Events.Account;
using Core.Services.Interfaces;
using Core.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Notification.Worker.Domain;
using Notification.Worker.Enumerations;
using Notification.Worker.Infrastructure;
using Notification.Worker.Repository;
using Notification.Worker.Services;
using Notification.Worker.Services.Interfaces;

namespace Notification.Worker.Consumers;
public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> logger,
    NotificationDbContext dbContext) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> _logger = logger;
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
         

        var target = await _userProfileRepository.GetUserByIdAsync(orderData.BuyerId);
        if (target is null || target.Profiles is null || target.Profiles.Count < 1)
        {
            return;
        }


        List<IdentityProfile> notFound = [];
        foreach (var profile in target.Profiles)
        {
            var fcmPayload = new
            {
                //notification = new 
                //{
                //    title = "Santai",
                //    body = $"Mechanic {orderData.MechanicName} has been assigned and will be heading to your location shortly",  
                //    click_action = "FLUTTER_NOTIFICATION_CLICK"
                //},
                to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"Mechanic {orderData.MechanicName} has been assigned and will be heading to your location shortly", 
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId,
                    mechanicName = orderData.MechanicName,
                    status = "MECHANIC_ORDER_ACCEPTED"
                }
            }; 

            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            { 
                @default = $"Mechanic {orderData.MechanicName} has been assigned and will be heading to your location shortly",
                GCM = Newtonsoft.Json.JsonConvert.SerializeObject(fcmPayload)  
            });
             
            var request = new PublishRequest
            {
                Message = messageJson,
                MessageStructure = "json", 
                TargetArn = profile.Arn 
            };

            try
            {
                await _messageService.PublishPushNotificationAsync(request);
            }
            catch (EndpointDisabledException ex)
            {
                LoggerHelper.LogError(_logger, ex);
                try
                {
                    await _messageService.DeregisterDevice(profile.Arn ?? string.Empty);
                }
                catch (Exception ex2)
                {
                    LoggerHelper.LogError(_logger, ex2);
                }
                notFound.Add(profile);
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                LoggerHelper.LogError(_logger, ex);
                try
                {
                    await _messageService.DeregisterDevice(profile.Arn ?? string.Empty);
                }
                catch (Exception ex2)
                {
                    LoggerHelper.LogError(_logger, ex2);
                }
                notFound.Add(profile);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex);
            }
        }
        foreach (var profile in notFound)
        {
            target.Profiles.Remove(profile);
        }
        _userProfileRepository.Update(target);
        await _dbContext.SaveChangesAsync();
    }
}
