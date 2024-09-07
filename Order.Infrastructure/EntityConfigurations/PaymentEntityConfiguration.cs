using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure.EntityConfigurations;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    { 
    }
}
