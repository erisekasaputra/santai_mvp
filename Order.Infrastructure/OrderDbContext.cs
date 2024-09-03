using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Order.Infrastructure;

public class OrderDbContext : DbContext
{
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
