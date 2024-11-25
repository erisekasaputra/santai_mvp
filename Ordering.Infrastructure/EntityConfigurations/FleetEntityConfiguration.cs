using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.FleetAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class FleetEntityConfiguration : IEntityTypeConfiguration<Fleet>
{
    public void Configure(EntityTypeBuilder<Fleet> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(e => e.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ImageUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(e => e.BasicInspections)
            .WithOne()
            .HasForeignKey(e => e.FleetAggregateId)
            .OnDelete(DeleteBehavior.Cascade);
           
        builder.HasMany(e => e.PreServiceInspections)
            .WithOne()
            .HasForeignKey(e => e.FleetAggregateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.JobChecklists)
            .WithOne()
            .HasForeignKey(e => e.FleetAggregateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Comment)
            .IsRequired();

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
    }
}
