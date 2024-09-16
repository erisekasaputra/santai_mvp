using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.NationalIdentityAggregate; 
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using Account.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Account.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountDbContext _context;
    private readonly IMediator _mediator;
    private IDbContextTransaction? _transaction; 
    public IUserRepository BaseUsers { get; }
    public IBusinessLicenseRepository BusinessLicenses { get; }
    public ICertificationRepository Certifications { get; }
    public IDrivingLicenseRepository DrivingLicenses { get; }
    public INationalIdentityRepository NationalIdentities { get; }
    public ILoyaltyProgramRepository LoyaltyPrograms { get; }
    public IReferralProgramRepository ReferralPrograms { get; }
    public IReferredProgramRepository ReferredPrograms { get; }
    public IStaffRepository Staffs { get; } 
    public IFleetRepository Fleets { get; }  

    public UnitOfWork(AccountDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
        BaseUsers = new UserRepository(context);
        BusinessLicenses = new BusinessLicenseRepository(context);
        Certifications = new CertificationRepository(context);
        DrivingLicenses = new DrivingLicenseRepository(context);   
        NationalIdentities = new NationalIdentityRepository(context);
        LoyaltyPrograms = new LoyaltyProgramRepository(context);
        ReferralPrograms = new ReferralProgramRepository(context);
        ReferredPrograms = new ReferredProgramRepository(context);
        Staffs = new StaffRepository(context);
        Fleets = new FleetRepository(context); 
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
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
        var domainEntities = _context.ChangeTracker
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
        var entries = _context.ChangeTracker.Entries();

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

        var changesResult = await _context.SaveChangesAsync(cancellationToken); 

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
