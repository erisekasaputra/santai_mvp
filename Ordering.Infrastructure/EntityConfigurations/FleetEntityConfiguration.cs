using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.OrderAggregate;

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

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
    }
}
