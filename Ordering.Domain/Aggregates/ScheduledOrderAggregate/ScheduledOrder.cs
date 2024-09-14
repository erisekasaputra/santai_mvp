using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.ScheduledOrderAggregate;

public class ScheduledOrder : Entity
{
    public Guid OrderId { get; init; }
    public DateTime ScheduledAt { get; init; }
    public bool IsEventProcessed { get; set; }
    public ScheduledOrder(
        Guid orderId,
        DateTime scheduledAt)
    {
        OrderId = orderId;
        ScheduledAt = scheduledAt;
        IsEventProcessed = false;
    }

    public void MarkAsProcessed()
    {
        IsEventProcessed = true;
    }
}
