using Account.Domain.Aggregates.OrderTaskAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class OrderTaskWaitingMechanicConfirmEntityConfiguration : IEntityTypeConfiguration<OrderTaskWaitingMechanicConfirm>
{
    public void Configure(EntityTypeBuilder<OrderTaskWaitingMechanicConfirm> e)
    {  
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.OrderId)
            .IsUnique();

        e.Property(p => p.OrderId)
            .IsRequired(true);

        e.Property(p => p.MechanicId)
            .IsRequired(true); 

        e.Property(e => e.ExpiredAt)
           .HasColumnType("datetime2")
           .IsRequired();

        e.Property(e => e.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired(); 

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction);
    }
}
