using Core.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;

namespace Order.Infrastructure.EntityConfigurations;

public class FeeEntityConfiguration : IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        builder.HasKey(p => p.Id); 

        builder.Property(e => e.PercentageOrValueType)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<PercentageOrValueType>(val))
            .IsRequired();
        
        builder.Property(e => e.FeeDescription)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<FeeDescription>(val))
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

        builder.OwnsOne(p => p.FeeAmount, buildAction =>
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
