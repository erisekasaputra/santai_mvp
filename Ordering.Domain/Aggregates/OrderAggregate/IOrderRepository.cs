namespace Ordering.Domain.Aggregates.OrderAggregate;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
    void Update(Order order); 
}
