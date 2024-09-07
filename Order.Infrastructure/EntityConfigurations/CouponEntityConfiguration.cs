using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure.EntityConfigurations;

public class CouponEntityConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    { 
    }
}
