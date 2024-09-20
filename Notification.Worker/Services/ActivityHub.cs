using Core.Services.Interfaces; 
using Microsoft.AspNetCore.SignalR;
using Notification.Worker.SeedWorks;
using Notification.Worker.Services.Interfaces;
namespace Notification.Worker.Services;

public class ActivityHub : Hub<IActivityClient>
{ 
    private readonly IUserInfoService _userInfoService;
    private readonly ICacheService _cacheService; 
    public ActivityHub(
        IUserInfoService userInfoService,
        ICacheService cacheService )
    { 
        _userInfoService = userInfoService;
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
        await _cacheService.SetAsync(CacheKey.GetUserCacheKey(userId), connectionId, TimeSpan.Zero); 
        await base.OnConnectedAsync();
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

