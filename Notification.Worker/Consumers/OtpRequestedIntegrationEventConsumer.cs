using Core.Events.Identity;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Core.Utilities;

namespace Notification.Worker.Consumers;

public class OtpRequestedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    ILogger<OtpRequestedIntegrationEventConsumer> logger) : IConsumer<OtpRequestedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly IMessageService _messageService = messageService;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger<OtpRequestedIntegrationEventConsumer> _logger = logger;
    
    public async Task Consume(ConsumeContext<OtpRequestedIntegrationEvent> context)
    {
        try
        {
            var otp = context.Message;

            if (otp.Provider == Core.Enumerations.OtpProviderType.Sms) 
            { 
                string template = $"Your token is: {otp.Token}. Please keep it confidential and do not share it with anyone. For your security, never disclose this token to others";
                await _messageService.PublishSmsAsync(otp.PhoneNumber, template); 
            }
        }
        catch (Exception ex) 
        {
            LoggerHelper.LogError(_logger, ex); 
        }
    } 
}
