namespace Account.API.Applications.Models;

public class MechanicExistence
{
    public required Guid MechanicId { get; set; }   
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public Guid? OrderId { get; set; } 
    public required string MechanicStatus { get; set; }
}
