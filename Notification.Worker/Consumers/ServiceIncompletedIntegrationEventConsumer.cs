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
using Notification.Worker.Infrastructure;
using Amazon.SimpleNotificationService;
using Core.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Notification.Worker.Domain;

namespace Notification.Worker.Consumers;

public class ServiceIncompletedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration,
    ILogger<ServiceIncompletedIntegrationEventConsumer> logger,
    NotificationDbContext dbContext, 
    INotificationService notificationService) : IConsumer<ServiceIncompletedIntegrationEvent>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<ServiceIncompletedIntegrationEventConsumer> _logger = logger;
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;
    public async Task Consume(ConsumeContext<ServiceIncompletedIntegrationEvent> context)
    {
        var orderData = context.Message; 
        await _activityHubContext.Clients.User(orderData.BuyerId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            orderData.MechanicId.ToString(),
            string.Empty,
            OrderStatus.ServiceIncompleted.ToString(),
            string.Empty);

        await _activityHubContext.Clients.User(orderData.MechanicId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            orderData.MechanicId.ToString(),
            String.Empty,
            OrderStatus.ServiceIncompleted.ToString(),
            string.Empty);

        try
        {
            await _notificationService.SaveNotification(new Notify(orderData.BuyerId.ToString(), NotifyType.Transaction.ToString(), "Order", $"Your service is complete, but the mechanic was unable to repair your vehicle"));
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }

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
                notification = new
                {
                    title = "SANTAIMOTO",
                    body = $"Your service is complete, but the mechanic was unable to repair your vehicle",
                    click_action = "FLUTTER_NOTIFICATION_CLICK"
                },
                to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken,
                    title = "SANTAIMOTO",
                    body = $"Your service is complete, but the mechanic was unable to repair your vehicle", 
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId,
                    status = "SERVICE_INCOMPLETED"
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                //@default = "Your service is complete, but the mechanic was unable to repair your vehicle",
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
