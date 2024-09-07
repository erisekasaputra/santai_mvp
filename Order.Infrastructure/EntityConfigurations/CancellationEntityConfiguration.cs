using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;

namespace Order.Infrastructure.EntityConfigurations;

public class CancellationEntityConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(EntityTypeBuilder<Cancellation> builder)
    { 
        builder.HasKey(p => p.Id);

        builder.HasMany(p => p.CancellationCharges)
            .WithOne()
            .HasForeignKey(p => p.OrderingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(p => p.CancellationRefund, buildAction =>
        {
            buildAction.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(18, 4);

            buildAction.Property(e => e.Currency)
                .HasConversion(
                    val => val.ToString(),
                    val => Enum.Parse<Currency>(val))
                .IsRequired();
        });

        builder.Ignore(p => p.DomainEvents);
    }
}
