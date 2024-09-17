using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.ScheduledOrderAggregate;

public class ScheduledOrder : Entity
{
    public Guid OrderId { get; init; }
    public DateTime ScheduledAt { get; set; } 
    public bool IsPaid { get; set; }
    public bool IsEventProcessed { get; set; }

    public ScheduledOrder(
        Guid orderId,
        DateTime scheduledAt,
        bool isShouldRequestPayment)
    {
        OrderId = orderId;
        ScheduledAt = scheduledAt;
        IsEventProcessed = false; 
        IsPaid = !isShouldRequestPayment;
    }

    public void SetNow()
    {
        ScheduledAt = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        if (!IsPaid) 
        {
            return;
        }

        IsPaid = true;
    }

    public void MarkAsProcessed()
    {
        IsEventProcessed = true;
    }
}
