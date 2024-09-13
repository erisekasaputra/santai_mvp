using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Aggregates.OrderTaskAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Account.Infrastructure;

public class AccountDbContext : DbContext
{
    public DbSet<BaseUser> BaseUsers { get; set; }
    public DbSet<BusinessLicense> BusinessLicenses { get; set; }
    public DbSet<DrivingLicense> DrivingLicenses { get; set; }
    public DbSet<NationalIdentity> NationalIdentities { get; set; }
    public DbSet<LoyaltyProgram> LoyaltyPrograms { get; set; }
    public DbSet<ReferralProgram> ReferralPrograms { get; set; }
    public DbSet<ReferredProgram> ReferredPrograms { get; set; }
    public DbSet<Certification> Certifications { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Fleet> Fleets { get; set; }
    public DbSet<MechanicOrderTask> MechanicOrderTasks { get; set; } 
    public DbSet<OrderTaskWaitingMechanicAssign> OrderTaskWaitingMechanicAssigns { get; set; } 
    public DbSet<OrderTaskWaitingMechanicConfirm> OrderTaskWaitingMechanicConfirms { get; set; } 
    public DbSet<MechanicOrderTaskAborted> MechanicOrderTaskAborteds { get; set; } 

    private IDbContextTransaction? _currentTransaction;

    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
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
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("Transaction has not been created yet.");
            }
            await _currentTransaction.RollbackAsync();
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<BusinessUser>("BusinessUser")
            .HasValue<RegularUser>("RegularUser")
            .HasValue<MechanicUser>("MechanicUser");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAccountInfrastructureMarkerInterface).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();

        base.OnModelCreating(modelBuilder);
    }
}
