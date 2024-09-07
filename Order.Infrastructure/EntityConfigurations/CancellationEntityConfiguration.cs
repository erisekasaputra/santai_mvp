using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure.EntityConfigurations;

public class CancellationEntityConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(EntityTypeBuilder<Cancellation> builder)
    { 
    }
}
