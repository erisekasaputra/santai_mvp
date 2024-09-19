using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ordering.Domain.Aggregates.CouponAggregate;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure.Repositories;
using System.Data;

namespace Ordering.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _dbContext;

    private readonly IMediator _mediator;

    private IDbContextTransaction? _transaction; 
    public IOrderRepository Orders { get; } 
    public IScheduledOrderRepository ScheduledOrders { get; }
    public ICouponRepository Coupons { get; }

    public UnitOfWork(OrderDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        Orders = new OrderRepository(dbContext);
        ScheduledOrders = new ScheduledOrderRepository(dbContext);
        Coupons = new CouponRepository(dbContext);
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("No transaction started.");
            }

            await SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch(Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public async Task DispatchDomainEventsAsync(CancellationToken token = default)
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity is Entity entity &&
                        entity.DomainEvents is not null &&
                        entity.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e =>
            {
                if (e.Entity.DomainEvents is null)
                {
                    return [];
                }
                return e.Entity.DomainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, token);
        }

        domainEntities.ForEach(e => e.Entity.ClearDomainEvents());
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("No transaction started.");
            }

            await _transaction.RollbackAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = _dbContext.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is Entity entity)
            {
                entry.State = entity.EntityStateAction switch
                {
                    EntityState.Detached => EntityState.Detached,
                    EntityState.Deleted => EntityState.Deleted,
                    EntityState.Modified => EntityState.Modified,
                    EntityState.Added => EntityState.Added,
                    _ => entry.State
                };

                entity.SetEntityState(EntityState.Unchanged);
            }
        }

        await DispatchDomainEventsAsync(cancellationToken);

        var changesResult = await _dbContext.SaveChangesAsync(cancellationToken);

        return changesResult;
    }
     
    public void Dispose()
    {
        if (_transaction != null)
        {
            RollbackTransactionAsync().GetAwaiter().GetResult();
        }
    }

    private void DisposeTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}
