using Core.Events.Catalog;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;
using Core.Utilities;
using Notification.Worker.Domain;
using Notification.Worker.Enumerations;
using Core.Configurations;
using Notification.Worker.Infrastructure;
using Notification.Worker.Repository;
using Microsoft.Extensions.Options;

namespace Notification.Worker.Consumers;

public class ItemPriceSetIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    IMessageService messageService,
    ICacheService cacheService,
    UserProfileRepository userProfileRepository,
    IOptionsMonitor<ProjectConfiguration> projectConfiguration,
    ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> logger,
    NotificationDbContext dbContext,
    INotificationService notificationService) : IConsumer<ItemPriceSetIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;

    private readonly INotificationService _notificationService = notificationService;
    private readonly NotificationDbContext _dbContext = dbContext;
    private readonly ILogger<AccountMechanicOrderAcceptedIntegrationEventConsumer> _logger = logger;
    private readonly IMessageService _messageService = messageService; 
    private readonly UserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly ProjectConfiguration _projectConfiguration = projectConfiguration.CurrentValue;


    public async Task Consume(ConsumeContext<ItemPriceSetIntegrationEvent> context)
    {  
        //try
        //{
        //    await _notificationService.SaveNotification(new Notify(orderData.BuyerId.ToString(), NotifyType.Transaction.ToString(), "Order", $"Mechanic {orderData.MechanicName} has been assigned and will be heading to your location shortly"));
        //}
        //catch (Exception ex)
        //{
        //    LoggerHelper.LogError(_logger, ex);
        //}
        await Task.CompletedTask;
    }
}
