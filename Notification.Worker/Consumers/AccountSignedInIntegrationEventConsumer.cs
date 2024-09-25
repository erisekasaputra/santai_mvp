using Amazon.SimpleNotificationService.Model;
using Core.Events.Identity;
using MassTransit;
using MassTransit.SqlTransport;
using Notification.Worker.Domain;
using Notification.Worker.Infrastructure;
using Notification.Worker.Repository;
using Notification.Worker.Services.Interfaces;

namespace Notification.Worker.Consumers;

public class AccountSignedInIntegrationEventConsumer : IConsumer<AccountSignedInIntegrationEvent>
{
    private readonly IMessageService _messageService;
    private readonly UserProfileRepository _userProfileRepository;
    private readonly NotificationDbContext _notificationDbContext;
    public AccountSignedInIntegrationEventConsumer(
        IMessageService messageService,
        UserProfileRepository userProfile,
        NotificationDbContext notificationDbContext)
    {
        _messageService = messageService;
        _userProfileRepository = userProfile;
        _notificationDbContext = notificationDbContext;
    }

    public async Task Consume(ConsumeContext<AccountSignedInIntegrationEvent> context)
    {
        try
        {
            var profile = context.Message;

            if (string.IsNullOrEmpty(profile.DeviceId))
            {
                return;
            }

            var userProfile = await _userProfileRepository.GetUserByIdAsync(profile.UserId);

            if (userProfile is null)
            {
                userProfile = new UserProfile(
                    profile.UserId,
                    profile.PhoneNumber,
                    profile.Email);

                var arnInt = await _messageService.RegisterDevice(profile.DeviceId);
                if (!string.IsNullOrEmpty(arnInt))
                {
                    userProfile.AddUserProfile(new IdentityProfile(profile.DeviceId, arnInt));
                }

                await _userProfileRepository.AddAsync(userProfile);
                await _notificationDbContext.SaveChangesAsync();
                return;
            }

            var arn = await _messageService.RegisterDevice(profile.DeviceId);
            if (!string.IsNullOrEmpty(arn))
            {
                userProfile.AddUserProfile(new IdentityProfile(profile.DeviceId, arn));
            }
            _userProfileRepository.Update(userProfile);
            await _notificationDbContext.SaveChangesAsync();
        }

        catch (InvalidParameterException ex)
        {
            if (ex.Message.Contains("DuplicateEndpoint"))
            {
                _ = ExtractEndpointArnFromExceptionMessage(ex.Message);
            }
            else
            {
                throw;
            }
        }
        catch (Exception) 
        {
            throw;
        } 
    }

    private static string? ExtractEndpointArnFromExceptionMessage(string message)
    { 
        var arnIndex = message.IndexOf("arn:aws:sns:");
        if (arnIndex >= 0)
        {
            return message[arnIndex..].Split(' ')[0];
        }
        return null;
    }
}
