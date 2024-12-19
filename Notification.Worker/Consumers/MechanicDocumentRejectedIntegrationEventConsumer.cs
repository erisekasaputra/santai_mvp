using Core.Configurations;
using Core.Events.Account;
using Core.Services.Interfaces; 
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Infrastructure;
using Notification.Worker.Repository;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Microsoft.Extensions.Options;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Core.Utilities;
using Notification.Worker.Domain;
using Notification.Worker.Enumerations;

namespace Notification.Worker.Consumers;

public class MechanicDocumentRejectedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    IOptionsMonitor<OrderConfiguration> orderConfiguration,
    ILogger<MechanicDocumentRejectedIntegrationEventConsumer> logger,
    NotificationDbContext dbContext,
    INotificationService notificationService) : IConsumer<MechanicDocumentRejectedIntegrationEvent>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<MechanicDocumentRejectedIntegrationEventConsumer> _logger = logger;
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly OrderConfiguration _orderConfiguration = orderConfiguration.CurrentValue;
  

    public async Task Consume(ConsumeContext<MechanicDocumentRejectedIntegrationEvent> context)
    {
        var orderData = context.Message;

        try
        {
            await _notificationService.SaveNotification(new Notify(orderData.MechanicId.ToString(), NotifyType.Information.ToString(), "Documents", $"Your documents have been reviewed by SANTAIMOTO but unfortunately, they were not approved."));
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }

        var target = await _userProfileRepository.GetUserByIdAsync(orderData.MechanicId);
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
                    body = $"Your documents have been reviewed by SANTAIMOTO but unfortunately, they were not approved.",
                    click_action = "FLUTTER_NOTIFICATION_CLICK"
                },
                to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken,
                    title = "SANTAIMOTO",
                    body = $"Your documents have been reviewed by SANTAIMOTO but unfortunately, they were not approved.",
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    orderId = orderData.MechanicId,
                    status = "SERVICE_STARTING"
                }
            };


            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                //@default = "Your vehicle service is starting",
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
