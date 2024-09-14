using Core.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Enumerations;

namespace Ordering.Infrastructure.EntityConfigurations;

public class DiscountEntityConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CouponCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.CouponValueType)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<PercentageOrValueType>(val))
            .IsRequired();

        builder.Property(e => e.Currency)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<Currency>(val))
            .IsRequired();

        builder.Property(e => e.ValuePercentage)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(e => e.ValueAmount)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(e => e.MinimumOrderValue)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.OwnsOne(p => p.DiscountAmount, buildAction =>
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

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
    }
}
