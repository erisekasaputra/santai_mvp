using Core.Events.Catalog;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Services.Interfaces;
using Notification.Worker.Services;

namespace Notification.Worker.Consumers;

public class ItemPriceSetIntegrationEventConsumer(
    IHubContext<ActivityHub, IActivityClient> activityHubContecxt,
    ICacheService cacheService) : IConsumer<ItemPriceSetIntegrationEvent>
{
    private readonly IHubContext<ActivityHub, IActivityClient> _activityHubContext = activityHubContecxt;
    private readonly ICacheService _cacheService = cacheService;
    public async Task Consume(ConsumeContext<ItemPriceSetIntegrationEvent> context)
    {
        await Task.CompletedTask;
    }
}
