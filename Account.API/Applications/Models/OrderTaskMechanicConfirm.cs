namespace Account.API.Applications.Models;

public class OrderTaskMechanicConfirm
{
    public required Guid OrderId { get; set; }
    public required Guid MechanicId { get; set; }
    public required DateTime ExpiredAtUtc { get; set; }
}
