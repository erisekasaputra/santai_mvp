using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;
using System.Data;

namespace Ordering.Domain.SeedWork;

public interface IUnitOfWork
{
    IOrderRepository Orders { get; }
    IScheduledOrderRepository ScheduledOrders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task DispatchDomainEventsAsync(CancellationToken token = default);
}
