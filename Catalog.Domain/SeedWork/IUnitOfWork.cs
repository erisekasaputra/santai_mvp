using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.ItemAggregate;
using System.Data;

namespace Catalog.Domain.SeedWork;

public interface IUnitOfWork
{
    IItemRepository Items { get; }

    ICategoryRepository Categories { get; }
    
    IBrandRepository Brands { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
    
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task DispatchDomainEventsAsync(CancellationToken token = default);
}
