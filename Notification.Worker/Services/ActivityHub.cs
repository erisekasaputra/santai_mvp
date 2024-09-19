using Core.Services.Interfaces; 
using Microsoft.AspNetCore.SignalR; 

namespace Notification.Worker.Services;

public class ActivityHub : Hub
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
        await base.OnConnectedAsync();
    }
}

