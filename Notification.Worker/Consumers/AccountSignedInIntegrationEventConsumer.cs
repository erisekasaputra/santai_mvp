using Amazon.SimpleNotificationService.Model;
using Core.Events.Identity;
using MassTransit; 
using Notification.Worker.Domain;
using Notification.Worker.Infrastructure;
using Notification.Worker.Repository;
using Notification.Worker.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

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
        IDbContextTransaction? transaction = null;
        try
        {
            transaction = await _notificationDbContext.Database.BeginTransactionAsync(); 
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
                await transaction.CommitAsync();
                return;
            }

            var arn = await _messageService.RegisterDevice(profile.DeviceId);
            if (!string.IsNullOrEmpty(arn))
            {
                userProfile.AddUserProfile(new IdentityProfile(profile.DeviceId, arn));
            }

            _userProfileRepository.Update(userProfile);
            await _notificationDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        } 
        catch (Exception)
        {
            if (transaction is not null) await transaction.RollbackAsync();
            throw;
        } 
    } 
}
