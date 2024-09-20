using Core.Events.Identity;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;

namespace Notification.Worker.Consumers;

public class OtpRequestedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<OtpRequestedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<OtpRequestedIntegrationEvent> context)
    {
        var orderData = context.Message; 


    }

    private async Task SendOtpByEmail()
    { 
    }
    private async Task SendOtpBySms()
    { 
    }
    private async Task SendOtpByWhatsapp()
    { 
    } 
}
