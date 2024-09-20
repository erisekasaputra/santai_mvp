using Core.Events.Account;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;

namespace Notification.Worker.Consumers;

public class BusinessLicenseRejectedIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<BusinessLicenseRejectedIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<BusinessLicenseRejectedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
