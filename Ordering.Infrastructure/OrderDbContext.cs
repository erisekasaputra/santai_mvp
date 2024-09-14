using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; 
using Ordering.Domain.Aggregates.BuyerAggregate;
using Ordering.Domain.Aggregates.CouponAggregate;
using Ordering.Domain.Aggregates.MechanicAggregate;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;
using System.Data;

namespace Ordering.Infrastructure;

public class OrderDbContext : DbContext
{
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<Discount> Discount { get; set; }
    public DbSet<Fee> Fees { get; set; }
    public DbSet<Fleet> Fleets { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ScheduledOrder> ScheduledOrders { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    private IDbContextTransaction? _currentTransaction;

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {

    }

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction is not null;

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IOrderInfrastructureMarkerInterface).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();

        base.OnModelCreating(modelBuilder);
    }
}
