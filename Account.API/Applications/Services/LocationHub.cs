using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Account.API.Applications.Services;

public class LocationHub : Hub
{
    private readonly IMechanicCache _cache;
    public LocationHub(IMechanicCache mechanicCache)
    {
        _cache = mechanicCache;
    }

    public async Task UpdateLocation(Guid UserId, double Latitude, double Longitude)
    { 
        var mechanic = new MechanicAvailabilityCache()
        {
            MechanicId = UserId,
            Latitude = Latitude,
            Longitude = Longitude
        };

        await _cache.UpdateLocationAsync(mechanic);  
    }
}
