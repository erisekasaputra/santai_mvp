using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.FleetAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class PreServiceInspectionResultEntityConfiguration : IEntityTypeConfiguration<PreServiceInspectionResult>
{
    public void Configure(EntityTypeBuilder<PreServiceInspectionResult> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderId)
            .IsRequired();

        builder.Property(o => o.FleetId)
            .IsRequired();

        builder.Property(o => o.PreServiceInspectionId)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(100)  
            .IsRequired(true); 

        builder.Property(o => o.Parameter)
            .HasMaxLength(100) 
            .IsRequired(true); 

        builder.Property(o => o.IsWorking)
            .IsRequired(); 

        builder.Ignore(e => e.DomainEvents);
    }
}
