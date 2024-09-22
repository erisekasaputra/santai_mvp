using Core.Events.Identity;
using Identity.API.Domain.Entities; 
using MassTransit;
using Microsoft.AspNetCore.Identity; 

namespace Identity.API.Consumers;

public class NotificationDeviceTokenRemovedIntegrationEventConsumer( 
    ILogger<NotificationDeviceTokenRemovedIntegrationEventConsumer> logger,
    UserManager<ApplicationUser> userManager) : IConsumer<NotificationDeviceTokenRemovedIntegrationEvent>
{  
    private readonly ILogger<NotificationDeviceTokenRemovedIntegrationEventConsumer> _logger = logger;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    public async Task Consume(ConsumeContext<NotificationDeviceTokenRemovedIntegrationEvent> context)
    {
        var user = await _userManager.FindByIdAsync(context.Message.UserId.ToString());
        if (user is null)
        {
            return;
        }

        user.RemoveDeviceId(context.Message.DeviceId); 
        await _userManager.UpdateAsync(user);
    }
}
