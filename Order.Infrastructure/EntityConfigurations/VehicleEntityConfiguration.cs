using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.EntityConfigurations;

public class VehicleEntityConfiguration : IEntityTypeConfiguration<VehicleEntityConfiguration>
{
    public void Configure(EntityTypeBuilder<VehicleEntityConfiguration> builder)
    {
        throw new NotImplementedException();
    }
}
