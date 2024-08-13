using Vehicle.API.Domain.Entities;

namespace Vehicle.API.Infrastructures.Repositories;

public class FleetRepository : IFleetRepository
{
    private readonly VehicleDbContext _vehicleDbContext;
    public FleetRepository(VehicleDbContext vehicleDbContext)
    {
        _vehicleDbContext = vehicleDbContext;
    }
}
