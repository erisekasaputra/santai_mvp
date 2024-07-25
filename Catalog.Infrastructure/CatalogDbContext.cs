using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.ItemAggregate; 
using Catalog.Infrastructure.EntityConfiguration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Catalog.Infrastructure;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options, IMediator mediator) : DbContext(options) 
{ 
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }

    private readonly IMediator _mediator = mediator;

    private IDbContextTransaction? _currentTransaction;

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {  
        _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }
    public async Task<IDbContextTransaction?> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_currentTransaction != null)
        {
            return null;
        }

        _currentTransaction = await Database.BeginTransactionAsync(isolationLevel);

        return _currentTransaction;
    }
    public async Task CommitTransactionAsync(IDbContextTransaction? transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction != _currentTransaction)
        {
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current.");
        }

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await RollBackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollBackTransaction()
    { 
        try
        {
            if ( _currentTransaction is null)
            {
                throw new ArgumentNullException($"Transaction has not been created yet.");
            }
            await _currentTransaction.RollbackAsync();
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ItemEntityConfigurator());

        modelBuilder.ApplyConfiguration(new CategoryEntityConfigurator());

        modelBuilder.ApplyConfiguration(new BrandEntityConfigurator());
        
        base.OnModelCreating(modelBuilder);
    }  
}
