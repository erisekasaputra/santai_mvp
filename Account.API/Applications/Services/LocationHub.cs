using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Core.Models;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.SignalR; 

namespace Account.API.Applications.Services;

public class LocationHub : Hub
{
    private readonly IMechanicCache _cache;
    private readonly IUserInfoService _userInfoService;
    private UserClaim? claims;
    public LocationHub(
        IMechanicCache mechanicCache,
        IUserInfoService userInfoService)
    {
        _cache = mechanicCache;
        _userInfoService = userInfoService;
    }

    public override Task OnConnectedAsync()
    {
        //claims = _userInfoService.GetUserInfo(); 
        //if (claims is null || claims.Sub == Guid.Empty || claims.CurrentUserType != Core.Enumerations.UserType.MechanicUser)
        //{
        //    Context.Abort(); 
        //} 

        return base.OnConnectedAsync();
    }


    public async Task UpdateLocation(double Latitude, double Longitude)
    {
        var user = _userInfoService.GetUserInfo(); 
        if (user is null)
        {
            Context.Abort(); 
            return; 
        } 

        var mechanic = new MechanicExistence()
        {
            MechanicId = user.Sub,
            Latitude = Latitude,
            Longitude = Longitude
        };

        var result = await _cache.UpdateLocationAsync(mechanic);   
        if (!result)
        {
            Context.Abort();    
        }
    }
}
