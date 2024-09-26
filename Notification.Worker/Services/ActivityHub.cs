using Microsoft.AspNetCore.SignalR;  
using Notification.Worker.Services.Interfaces;
namespace Notification.Worker.Services;

public class ActivityHub : Hub<IActivityClient>
{    
    public override async Task OnConnectedAsync()
    { 
        await base.OnConnectedAsync(); 
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    { 
        await base.OnDisconnectedAsync(exception);
    }
}

