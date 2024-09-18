using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.FleetAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class BasicInspectionEntityConfiguration : IEntityTypeConfiguration<BasicInspection>
{
    public void Configure(EntityTypeBuilder<BasicInspection> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderId)
            .IsRequired();

        builder.Property(o => o.FleetId)
            .IsRequired();

        builder.Property(o => o.FleetAggregateId)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(100) // Set a maximum length (adjust as needed)
            .IsRequired(true); // Optional field

        builder.Property(o => o.Parameter)
            .HasMaxLength(100) // Set a maximum length (adjust as needed)
            .IsRequired(true); // Optional field

        builder.Property(o => o.Value)
            .IsRequired();  

        builder.Ignore(e => e.DomainEvents);
    }
}
