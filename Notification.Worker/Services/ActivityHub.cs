using Core.Services.Interfaces; 
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.Enumerations;
using Notification.Worker.SeedWorks;
using Notification.Worker.Services.Interfaces;
namespace Notification.Worker.Services;

public class ActivityHub : Hub<IActivityClient>
{  
    private readonly ICacheService _cacheService; 
    public ActivityHub( 
        ICacheService cacheService )
    {  
        _cacheService = cacheService;  
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId == null) 
        {
            Context.Abort();
            return; 
        }
         
        var connectionId = Context.ConnectionId;

        await _cacheService.SetAsync(CacheKey.GetUserCacheKey(userId), connectionId, TimeSpan.FromHours(1));
        await base.OnConnectedAsync();
        await Clients.User(userId).ReceiveOrderStatusUpdate("test", "test", "test", "test", "test", OrderStatus.PaymentPending.ToString());
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        { 
            await _cacheService.DeleteAsync(CacheKey.GetUserCacheKey(userId));
        } 
        await base.OnDisconnectedAsync(exception);
    }
}

