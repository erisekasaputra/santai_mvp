using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.OrderTaskAggregate;

public class OrderTaskWaitingMechanicConfirm : Entity
{
    public Guid OrderId { get; private set; }
    public Guid MechanicId { get; private set; }
    public DateTime ExpirationAt { get; private set; } 
    public OrderTaskWaitingMechanicConfirm(
        Guid orderId,
        Guid mechanicId)
    {
        OrderId = orderId;
        MechanicId = mechanicId;
        ExpirationAt = DateTime.UtcNow.AddSeconds(60);
    }   
}
