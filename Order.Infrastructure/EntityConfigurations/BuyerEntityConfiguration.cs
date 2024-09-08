
using Core.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.BuyerAggregate; 

namespace Order.Infrastructure.EntityConfigurations;

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

        builder.Property(p => p.BuyerType)
            .IsRequired()
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<UserType>(val));

        builder.Ignore(p => p.DomainEvents);
    }
}
