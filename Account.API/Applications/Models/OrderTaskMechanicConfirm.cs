namespace Account.API.Applications.Models;

public class OrderTaskMechanicConfirm
{
    public string OrderId { get; private set; }
    public string MechanicId { get; private set; }
    public DateTime ExpiredAtUtc { get; private set; }

    public OrderTaskMechanicConfirm(string orderId, string mechanicId, DateTime expiredAtUtc)
    {
        OrderId = orderId;
        MechanicId = mechanicId;
        ExpiredAtUtc = expiredAtUtc;
    }
}
