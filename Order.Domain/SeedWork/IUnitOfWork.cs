using Order.Domain.Aggregates.OrderAggregate;
using System.Data;

namespace Order.Domain.SeedWork;

public interface IUnitOfWork
{  
    IOrderRepository Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task DispatchDomainEventsAsync(CancellationToken token = default); 
}
