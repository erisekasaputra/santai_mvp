using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.BuyerAggregate;
using Order.Domain.Aggregates.MechanicAggregate;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;

namespace Order.Infrastructure.EntityConfigurations;

public class OrderingEntityConfiguration : IEntityTypeConfiguration<Ordering>
{
    public void Configure(EntityTypeBuilder<Ordering> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(o => o.Buyer)
            .WithOne(p => p.Ordering)
            .HasForeignKey<Buyer>(o => o.OrderingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Mechanic)
            .WithOne(p => p.Ordering)
            .HasForeignKey<Mechanic>(o => o.OrderingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Payment)
           .WithOne(p => p.Ordering)
           .HasForeignKey<Payment>(o => o.OrderingId)
           .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(p => p.Coupon)
            .WithOne() 
            .HasForeignKey<Coupon>(e => e.OrderingId)
            .OnDelete(DeleteBehavior.Cascade);  






        builder.OwnsOne(p => p.Rating, buildAction =>
        {
            buildAction.Property(cp => cp.Value)
                .IsRequired(false);

            buildAction.Property(cp => cp.Comment)
                .IsRequired(false)
                .HasMaxLength(1000);
        }); 

        builder.OwnsOne(o => o.OrderAmount, buildAction =>
        {
            buildAction.Property(p => p.Amount)
                .IsRequired(true);

            buildAction.Property(p => p.Currency)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Currency>(v))
                .IsRequired(true);
        });
        
        builder.OwnsOne(o => o.GrandTotal, buildAction =>
        {
            buildAction.Property(p => p.Amount)
                .IsRequired(true);

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
            .HasForeignKey(p => p.OrderingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Fleets)
            .WithOne()
            .HasForeignKey(p => p.OrderingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Fees)
            .WithOne()
            .HasForeignKey(p => p.OrderingId)  
            .OnDelete(DeleteBehavior.Cascade);







        builder.Property(o => o.RatingImages)
            .HasConversion(
                v => v == null || !v.Any() ? string.Empty : string.Join(',', v),
                v => string.IsNullOrEmpty(v) ? new List<string>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .IsUnicode(false);

        builder.Property(o => o.BaseCurrency)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<Currency>(v));

        builder.Property(o => o.CreatedAtUtc)
            .IsRequired(); 

        builder.Property(o => o.IsScheduled)
            .IsRequired();

        builder.Property(o => o.TotalCanceledByMechanic)
            .HasDefaultValue(0);





        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsPaid);
        builder.Ignore(x => x.IsRated);
    }
}
