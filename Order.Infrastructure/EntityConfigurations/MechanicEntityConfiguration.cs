using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.MechanicAggregate;

namespace Order.Infrastructure.EntityConfigurations;

public class MechanicEntityConfiguration : IEntityTypeConfiguration<Mechanic>
{
    public void Configure(EntityTypeBuilder<Mechanic> builder)
    {
        throw new NotImplementedException();
    }
}
