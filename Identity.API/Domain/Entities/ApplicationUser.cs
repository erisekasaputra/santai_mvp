
using Core.Enumerations; 
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Domain.Entities;

public class ApplicationUser : IdentityUser
{  
    public bool IsAccountRegistered { get; set; }   
    public string? BusinessCode { get; set; } 
    public required UserType UserType { get; set; }
    public ICollection<string> DeviceIds { get; set; } = [];

    public bool AddDeviceId(string deviceId)
    {
        if(DeviceIds.Contains(deviceId))
        {
            return false;
        }

        DeviceIds.Add(deviceId);
        return true;
    }

    public bool RemoveDeviceId(string deviceId) 
    {
        if (!DeviceIds.Contains(deviceId)) 
        {
            return false;
        }

        DeviceIds.Remove(deviceId);
        return true;
    }
}
