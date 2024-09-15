using Account.Domain.Aggregates.OrderTaskAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.EntityConfigurations;

public class MechanicOrderTaskEnttiyConfiguration : IEntityTypeConfiguration<MechanicOrderTask>
{
    public void Configure(EntityTypeBuilder<MechanicOrderTask> e)
    {
        e.HasKey(p => p.Id);

        e.HasIndex(p => p.OrderId).IsUnique();

        e.Property(p => p.OrderId).IsRequired(false);
         
        e.Property(p => p.Longitude)
            .HasColumnType("float(24)");
         
        e.Property(p => p.Latitude)
            .HasColumnType("float(24)");

        e.Ignore(p => p.DomainEvents);
        e.Ignore(p => p.EntityStateAction); 
    }
}
