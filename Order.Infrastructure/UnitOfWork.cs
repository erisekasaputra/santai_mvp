using Amazon.Util.Internal;
using Core.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.SeedWork;
using Order.Infrastructure.Repositories;
using System.Data;

namespace Order.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _dbContext;

    private readonly IMediator _mediator;
    
    private IDbContextTransaction? _transaction;
    
    public IOrderRepository Orders { get; }

    public UnitOfWork(OrderDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        Orders = new OrderRepository(dbContext);
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
        catch
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
                    EntityStateAction.Deleted => EntityState.Deleted,
                    EntityStateAction.Modified => EntityState.Modified,
                    EntityStateAction.Added => EntityState.Added,
                    _ => entry.State
                };

                entity.SetEntityState(EntityStateAction.NoAction);
            }
        } 

        await DispatchDomainEventsAsync(cancellationToken);

        var changesResult = await _dbContext.SaveChangesAsync(cancellationToken); 

        var domainEntities = _dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents is not null && e.Entity.DomainEvents.Count > 0)
            .ToList();

        domainEntities.ForEach(e => e.Entity.ClearDomainEvents()); 

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
