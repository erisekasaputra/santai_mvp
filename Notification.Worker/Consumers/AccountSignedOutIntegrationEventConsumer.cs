using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Amazon;
using Core.Events.Identity;
using Core.Utilities;
using MassTransit; 
using Notification.Worker.Infrastructure;
using Notification.Worker.Repository;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;

namespace Notification.Worker.Consumers;

public class AccountSignedOutIntegrationEventConsumer : IConsumer<AccountSignedOutIntegrationEvent>
{
    private readonly IMessageService _messageService;
    private readonly UserProfileRepository _userProfileRepository;
    private readonly NotificationDbContext _notificationDbContext;
    private readonly ILogger<AccountSignedOutIntegrationEventConsumer> _logger;
    private readonly INotificationService _notificationService;
    public AccountSignedOutIntegrationEventConsumer(
        IMessageService messageService,
        UserProfileRepository userProfile,
        NotificationDbContext notificationDbContext,
        ILogger<AccountSignedOutIntegrationEventConsumer> logger, 
        INotificationService notificationService)
    {
        _messageService = messageService;
        _userProfileRepository = userProfile;
        _notificationDbContext = notificationDbContext;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<AccountSignedOutIntegrationEvent> context)
    {
        try
        {
            var profile = context.Message;

            if (string.IsNullOrEmpty(profile.DeviceId))
            {
                return;
            }

            var userProfile = await _userProfileRepository.GetUserByIdAsync(profile.UserId); 
            if (userProfile is null || userProfile.Profiles is null)
            {
                return;
            }

            string? arn = userProfile.Profiles.Where(x => x.DeviceToken.Trim() == profile.DeviceId.Trim()).Select(x => x.Arn).FirstOrDefault();
            if (string.IsNullOrEmpty(arn)) 
            {
                return;
            }

            var profileToRemove = userProfile.Profiles.FirstOrDefault(x => x.DeviceToken.Trim() == profile.DeviceId.Trim()); 
            if (profileToRemove is null)
            {
                return;  // Tidak ada profil yang cocok
            }
             
            userProfile.RemoveUserProfile(profileToRemove); 
            _userProfileRepository.Update(userProfile);

            await _messageService.DeregisterDevice(arn);
            await _notificationDbContext.SaveChangesAsync();
        } 
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }
    }
}
