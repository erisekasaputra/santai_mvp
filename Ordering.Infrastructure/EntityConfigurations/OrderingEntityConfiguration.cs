using Core.Enumerations;
using Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Enumerations;

namespace Ordering.Infrastructure.EntityConfigurations;

public class OrderingEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(o => o.Buyer)
            .WithOne(p => p.Order)
            .HasForeignKey<Buyer>(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Mechanic)
            .WithOne(p => p.Order)
            .HasForeignKey<Mechanic>(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Payment)
           .WithOne(p => p.Order)
           .HasForeignKey<Payment>(o => o.OrderId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Discount)
            .WithOne()
            .HasForeignKey<Discount>(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.Currency)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<Currency>(v));

        builder.OwnsOne(p => p.Rating, buildAction =>
        {
            buildAction.Property(cp => cp.Value)
                .IsRequired()
                .HasPrecision(18, 4);

            buildAction.Property(cp => cp.Comment)
                .IsRequired(false)
                .HasMaxLength(1000);
        });

        builder.Property(p => p.OrderAmount)
            .IsRequired(true)
            .HasPrecision(18, 4);

        builder.Property(p => p.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<OrderStatus>(v))
            .IsRequired(true);

        builder.OwnsOne(o => o.GrandTotal, buildAction =>
        {
            buildAction.Property(p => p.Amount)
                .IsRequired(true)
                .HasPrecision(18, 4);

            buildAction.Property(p => p.Currency)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Currency>(v))
                .IsRequired(true);
        });

        builder.OwnsOne(o => o.Address, buildAction =>
        {
            buildAction.Property(p => p.AddressLine)
                .IsRequired(true);

            buildAction.Property(p => p.Latitude)
                .IsRequired(true);

            buildAction.Property(p => p.Longitude)
                .IsRequired(true);
        });


        builder.HasMany(o => o.LineItems)
            .WithOne()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Fleets)
            .WithOne()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Fees)
            .WithOne()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        var converter = new ValueConverter<ICollection<string>?, string>(
             v => CustomSerializer.SerializeList(v),
             v => CustomSerializer.DeserializeList(v)
        );

        var comparer = new ValueComparer<ICollection<string>?>(
            (c1, c2) =>
                c1 == null && c2 == null || c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c == null ? null : c.ToList()
        );

        builder.Property(o => o.RatingImages)
            .HasConversion(converter)
            .IsUnicode(false)
            .Metadata.SetValueComparer(comparer);

        builder.HasOne(o => o.Cancellation)
            .WithOne()
            .HasForeignKey<Cancellation>(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Property(o => o.ScheduledOnUtc)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(o => o.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");


        builder.Property(o => o.IsScheduled)
            .IsRequired();

        builder.Property(o => o.TotalCanceledByMechanic)
            .HasDefaultValue(0); 

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsShouldRequestPayment);
        builder.Ignore(x => x.IsPaid);
        builder.Ignore(x => x.IsRated);
        builder.Ignore(x => x.IsPaymentExpire);
        builder.Ignore(x => x.PaymentUrl);
    }
}
