
using Ordering.Domain.Enumerations;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
    Task<(int TotalCount, int TotalPages, IEnumerable<Order> Orders)> GetPaginatedOrders(Guid? userId, int pageNumber, int pageSize, OrderStatus? status);
    Task<Order?> GetByIdAndUserIdNoTrackingAsync(Guid orderId, Guid buyerId, CancellationToken cancellationToken);
    Task<string?> GetOrderSecretByOrderIdAndUserId(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task <IEnumerable<Order>> GetOrderServicesActive(Guid? userId);
}
