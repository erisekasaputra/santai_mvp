using System.Data;

namespace Account.Domain.SeedWork;

public interface IUnitOfWork
{
    //IItemRepository Items { get; }
     

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task DispatchDomainEventsAsync(CancellationToken token = default);
}
