using Account.API.Applications.Models;

namespace Account.API.Applications.Services.Interfaces;

public interface IMechanicCache
{
    Task UpdateLocationAsync(MechanicAvailabilityCache mechanic);
    Task CreateGeoAsync(MechanicAvailabilityCache mechanic);
    Task RemoveGeoAsync(Guid mechanicId);
    Task RemoveHsetAsync(Guid mechanicId);
    Task CreateMechanicHsetAsync(MechanicAvailabilityCache mechanic);
    Task<Guid> AssignOrderToMechanicAsync(MechanicAvailabilityCache mechanic, Guid orderId);
    Task<MechanicAvailabilityCache?> FindAvailableMechanicAsync(
        double latitude, double longitude, double radius);
    Task<MechanicAvailabilityCache?> GetMechanicAsync(Guid mechanicId);
}
