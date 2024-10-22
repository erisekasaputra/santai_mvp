using Core.Enumerations;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(e => e.PaymentMethod)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.BankReference)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasColumnType("datetime2")
           .HasConversion(DateTimeExtension.UtcConverter());

        builder.Property(e => e.TransactionAt)
           .IsRequired(false)
           .HasColumnType("datetime2")
           .HasConversion(DateTimeExtension.UtcConverter());

        builder.OwnsOne(p => p.Amount, buildAction =>
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
