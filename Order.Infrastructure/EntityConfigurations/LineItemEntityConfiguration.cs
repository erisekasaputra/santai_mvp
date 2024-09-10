using Core.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;

namespace Order.Infrastructure.EntityConfigurations;

public class LineItemEntityConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(e => e.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Quantity)
            .IsRequired();

        builder.Property(e => e.UnitPrice)
               .IsRequired()
               .HasPrecision(18, 4);

        builder.OwnsOne(p => p.SubTotal, buildAction =>
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

        builder.OwnsOne(p => p.Tax, buildAction =>
        {
            buildAction.Property(e => e.Rate)
                .IsRequired()
                .HasPrecision(18, 4); 

            buildAction.OwnsOne(e => e.TaxAmount, taxBuildAction =>
            {
                taxBuildAction.Property(e => e.Amount)
                    .IsRequired()
                    .HasPrecision(18, 4);

                taxBuildAction.Property(e => e.Currency)
                    .HasConversion(
                        val => val.ToString(),
                        val => Enum.Parse<Currency>(val))
                    .IsRequired();
            });
        });


        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
    }
}
