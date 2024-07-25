using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;
using Catalog.Infrastructure.Repositories; 
using Microsoft.EntityFrameworkCore.Storage; 

namespace Catalog.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    public readonly CatalogDbContext _context;
    
    public IItemRepository Items => _itemRepository ??= new ItemRepository(_context);

    public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);

    public IItemRepository _itemRepository;

    public ICategoryRepository _categoryRepository;

    private IDbContextTransaction? _dbTransactionContext;

    public UnitOfWork(CatalogDbContext context)
    {
        _context = context;
        _itemRepository = new ItemRepository(_context);
        _categoryRepository = new CategoryRepository(_context);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _dbTransactionContext = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(_dbTransactionContext);

            await SaveChangesAsync(cancellationToken);

            await _dbTransactionContext.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_dbTransactionContext);

        await _dbTransactionContext.RollbackAsync(cancellationToken);
    }
}
