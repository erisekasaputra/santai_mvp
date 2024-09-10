namespace Order.Domain.Aggregates.OrderAggregate;

public interface IOrderRepository
{
    Task<Ordering?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Ordering> CreateAsync(Ordering order, CancellationToken cancellationToken);
    void Update(Ordering order);
    
}
