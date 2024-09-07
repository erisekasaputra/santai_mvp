using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure.EntityConfigurations;

public class FleetEntityConfiguration : IEntityTypeConfiguration<Fleet>
{
    public void Configure(EntityTypeBuilder<Fleet> builder)
    { 
    }
}
