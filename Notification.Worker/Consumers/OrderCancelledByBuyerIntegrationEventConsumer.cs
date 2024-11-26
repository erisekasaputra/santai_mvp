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
using Amazon.SimpleNotificationService;
using Core.Utilities;
using Notification.Worker.Infrastructure;
using Notification.Worker.Domain;

namespace Notification.Worker.Consumers;

public class OrderCancelledByBuyerIntegrationEventConsumer(
IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration,
    ILogger<OrderCancelledByBuyerIntegrationEventConsumer> logger,
    NotificationDbContext dbContext) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<OrderCancelledByBuyerIntegrationEventConsumer> _logger = logger;
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

        if (orderData.MechanicId is null || !orderData.MechanicId.HasValue)
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


        Console.WriteLine(orderData.MechanicId.Value);
        var target = await _userProfileRepository.GetUserByIdAsync(orderData.MechanicId.Value);
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
                //    body = $"Uhh, Order has been canceled by customer",
                //    click_action = "FLUTTER_NOTIFICATION_CLICK"
                //},
                //to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"Uhh, Order has been canceled by customer", 
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId.Value,
                    status = "ORDER_CANCELLED_BY_BUYER"
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                //@default = "Uhh, Order has been canceled by customer",
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
