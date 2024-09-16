namespace Account.API.Applications.Models;

public class OrderTask
{
    public required Guid BuyerId { get; set; }
    public required Guid OrderId { get; set; }
    public Guid? MechanicId { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required string OrderStatus { get; set; }
}
