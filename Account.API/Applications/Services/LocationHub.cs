using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;  
using Microsoft.AspNetCore.SignalR; 

namespace Account.API.Applications.Services;

public class LocationHub : Hub
{
    private readonly IMechanicCache _cache; 
    public LocationHub(
        IMechanicCache mechanicCache )
    {
        _cache = mechanicCache; 
    }

    public override Task OnConnectedAsync()
    { 
        return base.OnConnectedAsync();
    }


    public async Task UpdateLocation(double latitude, double longitude)
    {
        var mechanicId = Context.UserIdentifier;  
        if (mechanicId is null)
        {
            Context.Abort();
            return;
        }

        var mechanic = new MechanicExistence(
            mechanicId,
            string.Empty,
            latitude,
            longitude,
            MechanicStatus.Available);


        bool isSucceed = false;
        const int maxRetry = 3;
        int retryCount = 0;
        while (!isSucceed && retryCount < maxRetry)
        {
            isSucceed = await _cache.UpdateLocationAsync(mechanic); 
            if (!isSucceed)
            {
                retryCount++;
                await Task.Delay(100);
            } 
        }

        if (!isSucceed) 
        {
            Context.Abort();
        }
    }
}
