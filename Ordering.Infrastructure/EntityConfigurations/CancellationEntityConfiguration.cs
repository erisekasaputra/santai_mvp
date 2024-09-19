using Core.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class CancellationEntityConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(EntityTypeBuilder<Cancellation> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasMany(p => p.CancellationCharges)
            .WithOne()
            .HasForeignKey(p => p.CancellationId)
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

        builder.Property(e => e.IsRefundPaid)
            .IsRequired();

        builder.Property(e => e.ShouldRefundAtUt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
    }
}
