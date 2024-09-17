using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces; 
using Core.Services.Interfaces;
using Microsoft.AspNetCore.SignalR; 

namespace Account.API.Applications.Services;

public class LocationHub : Hub
{
    private readonly IMechanicCache _cache;
    private readonly IUserInfoService _userInfoService; 
    public LocationHub(
        IMechanicCache mechanicCache,
        IUserInfoService userInfoService)
    {
        _cache = mechanicCache;
        _userInfoService = userInfoService;
    }

    public override Task OnConnectedAsync()
    { 
        return base.OnConnectedAsync();
    }


    public async Task UpdateLocation(double latitude, double longitude)
    {
        var user = _userInfoService.GetUserInfo(); 
        if (user is null)
        {
            Context.Abort(); 
            return; 
        }

        var mechanic = new MechanicExistence(user.Sub.ToString(), string.Empty, latitude, longitude, MechanicStatus.Available);

        var result = await _cache.UpdateLocationAsync(mechanic);   
        if (!result)
        {
            Context.Abort();    
        }
    }
}
