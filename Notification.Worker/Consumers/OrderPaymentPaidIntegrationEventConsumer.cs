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
using Notification.Worker.Domain;

namespace Notification.Worker.Consumers;

public class OrderPaymentPaidIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration,
    ILogger<OrderPaymentPaidIntegrationEventConsumer> logger,
    NotificationDbContext dbContext) : IConsumer<OrderPaymentPaidIntegrationEvent>
{
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<OrderPaymentPaidIntegrationEventConsumer> _logger = logger;
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;

    public async Task Consume(ConsumeContext<OrderPaymentPaidIntegrationEvent> context)
    { 
        var orderData = context.Message; 
        await _activityHubContext.Clients.User(orderData.BuyerId.ToString()).ReceiveOrderStatusUpdate(
            orderData.OrderId.ToString(),
            orderData.BuyerId.ToString(),
            string.Empty,
            string.Empty,
            string.Empty,
            OrderStatus.PaymentPaid.ToString(),
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
                notification = new
                {
                    title = "Santai",
                    body = $"Your payment has been successfully received. Thank you for your trust and support",
                    click_action = "FLUTTER_NOTIFICATION_CLICK"
                },
                to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken,
                    title = "Santai",
                    body = $"Your payment has been successfully received. Thank you for your trust and support.", 
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    orderId = orderData.OrderId,
                    buyerId = orderData.BuyerId,
                    status = "PAYMENT_PAID"
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                @default = "Your payment has been successfully received. Thank you for your trust and support",
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
