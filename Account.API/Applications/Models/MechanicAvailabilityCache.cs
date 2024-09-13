namespace Account.API.Applications.Models;

public class MechanicAvailabilityCache
{
    public Guid MechanicId { get; set; }   
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid? OrderId { get; set; } 
}
