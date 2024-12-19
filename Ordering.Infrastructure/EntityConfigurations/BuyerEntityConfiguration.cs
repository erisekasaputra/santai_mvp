
using Core.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

public class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.BuyerId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Email)
           .IsRequired(false)
           .HasMaxLength(255);

        builder.Property(p => p.PhoneNumber)
           .IsRequired(false)
           .HasMaxLength(40);

        builder.Property(p => p.BuyerType)
            .IsRequired()
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<UserType>(val));

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
    }
}
