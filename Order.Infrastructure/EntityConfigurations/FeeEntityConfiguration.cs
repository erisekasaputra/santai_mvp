using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure.EntityConfigurations;

public class FeeEntityConfiguration : IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    { 
    }
}
