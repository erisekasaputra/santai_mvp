﻿using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.ItemAggregate;

namespace Catalog.Domain.SeedWork;

public interface IUnitOfWork
{
    IItemRepository Items { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
