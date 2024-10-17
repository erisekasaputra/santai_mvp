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

namespace Notification.Worker.Consumers;

public class OrderMechanicArrivedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration,
    ILogger<OrderMechanicArrivedIntegrationEventConsumer> logger,
    NotificationDbContext dbContext) : IConsumer<OrderMechanicArrivedIntegrationEvent>
{
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<OrderMechanicArrivedIntegrationEventConsumer> _logger = logger;
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;

    public async Task Consume(ConsumeContext<OrderMechanicArrivedIntegrationEvent> context)
    {
        var orderData = context.Message; 
        await _activityHubContext.Clients.User(orderData.BuyerId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            orderData.MechanicId.ToString(),
            orderData.MechanicName,
            OrderStatus.MechanicArrived.ToString(),
            string.Empty);

        var target = await _userProfileRepository.GetUserByIdAsync(orderData.BuyerId);
        if (target is null || target.Profiles is null || target.Profiles.Count < 1)
        {
            return;
        }

        foreach (var profile in target.Profiles)
        {
            var fcmPayload = new
            {
                notification = new
                {
                    title = "Santai",
                    body = $"The mechanic has arrived at your location, help them get more detailed directions to your location",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP"
                },
                to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken, 
                    title = "Santai",
                    body = $"The mechanic has arrived at your location, help them get more detailed directions to your location",
                    image = _projectConfiguration.LogoUrl,
                    click_action = "OPEN_APP",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    mechanicId = orderData.MechanicId
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                @default = "Mechanic has arrived at your location",
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
                target.RemoveUserProfile(profile);
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
                target.RemoveUserProfile(profile);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex);
            }
        }

        _userProfileRepository.Update(target);
        await _dbContext.SaveChangesAsync();
    }
}
