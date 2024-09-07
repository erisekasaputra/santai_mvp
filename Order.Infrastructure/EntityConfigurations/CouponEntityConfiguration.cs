using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;

namespace Order.Infrastructure.EntityConfigurations;

public class CouponEntityConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
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
       
        builder.Property(e => e.Percentage) 
            .IsRequired()
            .HasPrecision(18, 4);

        builder.OwnsOne(p => p.Value, buildAction =>
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

        builder.OwnsOne(p => p.MinimumOrderValue, buildAction =>
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


        builder.Ignore(p => p.DomainEvents);
    }
}
