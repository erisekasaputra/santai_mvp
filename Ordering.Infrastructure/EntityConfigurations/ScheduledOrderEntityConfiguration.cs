using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class ScheduledOrderEntityConfiguration : IEntityTypeConfiguration<ScheduledOrder>
{
    public void Configure(EntityTypeBuilder<ScheduledOrder> builder)
    { 
        builder.HasIndex(e => e.OrderId).IsUnique();

        builder.HasIndex(e => e.ScheduledAt); 

        builder.Property(o => o.ScheduledAt)
          .IsRequired()
          .HasColumnType("datetime2");


        builder.Ignore(e => e.DomainEvents);
        builder.Ignore(e => e.EntityStateAction);
    }
}
