using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Aggregates.BuyerAggregate;
using Order.Domain.Aggregates.MechanicAggregate;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure;

public class OrderDbContext : DbContext
{
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Fee> Fees { get; set; }
    public DbSet<Fleet> Fleets { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Ordering> Orderings { get; set; }
    public DbSet<Payment> Payments { get; set; } 

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
        
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
