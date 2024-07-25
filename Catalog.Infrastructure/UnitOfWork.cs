using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;
using Catalog.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage; 
namespace Catalog.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    public readonly CatalogDbContext _context;
    
    public IItemRepository Items => _itemRepository ??= new ItemRepository(_context);

    public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);

    public IBrandRepository Brands => _brandRepository ??= new BrandRepository(_context); 

    public IItemRepository _itemRepository;

    public ICategoryRepository _categoryRepository;

    public IBrandRepository _brandRepository;

    private IDbContextTransaction? _dbTransactionContext;

    private readonly IMediator _mediator;

    public UnitOfWork(CatalogDbContext context, IMediator mediator)
    {
        _context = context;
        _itemRepository = new ItemRepository(_context);
        _categoryRepository = new CategoryRepository(_context);
        _brandRepository = new BrandRepository(_context);
        _mediator = mediator;
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

    public async Task DispatchDomainEventsAsync(CancellationToken token = default)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity is Entity entity &&
                        entity.DomainEvents is not null &&
                        entity.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e =>
            { 
                if (e is null || e.Entity is null || e.Entity.DomainEvents is null)
                {
                    return [];
                }
                return e.Entity.DomainEvents;
            })
            .ToList();

        domainEntities.ForEach(e => ((Entity)e.Entity).ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, token);
        }
    }
}
