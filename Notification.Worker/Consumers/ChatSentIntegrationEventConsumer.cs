using Core.Configurations;
using Core.Events.Chat;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Notification.Worker.Repository;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Core.Utilities;
using Notification.Worker.Infrastructure;
using Notification.Worker.Domain;

namespace Notification.Worker.Consumers;

public class ChatSentIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    ILogger<ChatSentIntegrationEventConsumer> logger,
    NotificationDbContext dbContext,
    INotificationService notificationService) : IConsumer<ChatSentIntegrationEvent>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<ChatSentIntegrationEventConsumer> _logger = logger;
    private readonly IMessageService _messageService = messageService;
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;
    private readonly NotificationDbContext _dbContext = dbContext;
    public async Task Consume(ConsumeContext<ChatSentIntegrationEvent> context)
    { 
        var target = await _userProfileRepository.GetUserByIdAsync(context.Message.DestinationUserId);
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
                    body = context.Message.Text
                },
                to = profile.DeviceToken,
                data = new
                {
                    token = profile.DeviceToken,
                    title = "SANTAIMOTO",
                    body = context.Message.Text, 
                    click_action = "FLUTTER_NOTIFICATION_CLICK", 
                    messageId = context.Message.MessageId,
                    timestamp = context.Message.Timestamp,
                    originUserId = context.Message.OriginUserId,
                    status = "CHAT_SENT"
                }
            };

            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                //@default = context.Message.Text,
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
