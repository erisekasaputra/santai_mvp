namespace Ordering.Domain.Aggregates.ScheduledOrderAggregate;

public interface IScheduledOrderRepository
{
    Task<IEnumerable<ScheduledOrder>> GetOnTimeOrderWithUnpublishedEvent(int rowNumber);
    void UpdateOnTimeOrderWithPublishedEvent(IEnumerable<ScheduledOrder> orders);
    Task CraeteAsync(ScheduledOrder order);
    Task<ScheduledOrder?> GetByOrderIdAsync(Guid orderId);
    void Update(ScheduledOrder order);
}
