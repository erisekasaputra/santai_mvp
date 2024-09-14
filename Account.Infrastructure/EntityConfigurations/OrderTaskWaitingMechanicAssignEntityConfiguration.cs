using Account.Domain.Aggregates.OrderTaskAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class OrderTaskWaitingMechanicAssignEntityConfiguration : IEntityTypeConfiguration<OrderTaskWaitingMechanicAssign>
{
    public void Configure(EntityTypeBuilder<OrderTaskWaitingMechanicAssign> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.OrderId)
            .IsUnique();

        e.Property(p => p.OrderId)
            .IsRequired(true);

        e.Property(p => p.MechanicId)
            .IsRequired(false);

        e.Property(p => p.Longitude)
            .HasColumnType("float(24)");

        e.Property(p => p.Latitude)
            .HasColumnType("float(24)");

        e.Property(e => e.MechanicConfirmationExpire) 
           .HasColumnType("datetime2") 
           .IsRequired(false); 
         
        e.Property(e => e.CreatedAt) 
            .HasColumnType("datetime2") 
            .IsRequired(true);

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);  
    }
}
