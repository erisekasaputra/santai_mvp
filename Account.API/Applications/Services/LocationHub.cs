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

        string[] ids = {
                "2cfb79f6-8992-4fc8-bd7d-3e7145adf322",
                "367d4274-f3b9-456f-9019-ed09610d68eb",
                "6a60aca8-eef0-4b4a-8a73-a98b1272d671",
                "7c051a23-d2ad-44a5-8091-44382d8f5ab1",
                "7035b81a-ee66-4209-b505-ef6efbdd2881",
                "e85170c1-883c-42e7-8ba0-93b0e412271b",
                "a654d2af-4002-42ae-b8ef-b020254e636e",
                "f65eb0dc-5d4b-48d3-946c-22056989dcc5",
                "43f8590f-9a96-44fa-b151-65069ba6ff6d",
                "1718a5e1-0b90-43ff-b4a2-13f6eaaa5096"
        };


        foreach (var id in ids) 
        {
            var mechanic = new MechanicExistence(id, string.Empty, latitude, longitude, MechanicStatus.Available); 
            var result = await _cache.UpdateLocationAsync(mechanic);   
            if (!result)
            {
                //Context.Abort();    
            } 
        }
    }
}
