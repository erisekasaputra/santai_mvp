using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;
 
namespace Order.Infrastructure.EntityConfigurations;

public class OrderingEntityConfiguration : IEntityTypeConfiguration<Ordering>
{
    public void Configure(EntityTypeBuilder<Ordering> builder)
    { 
    }
}
