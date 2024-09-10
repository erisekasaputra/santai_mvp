using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Aggregates.MechanicAggregate; 

namespace Order.Infrastructure.EntityConfigurations;

public class MechanicEntityConfiguration : IEntityTypeConfiguration<Mechanic>
{
    public void Configure(EntityTypeBuilder<Mechanic> builder)
    {
        builder.HasKey(p => p.Id);
         
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Performance)
            .IsRequired()
            .HasPrecision(18, 4);




        builder.OwnsOne(p => p.Rating, buildAction =>
        {
            buildAction.Property(e => e.Value)
                .IsRequired() 
                .HasPrecision(18, 4);

            buildAction.Property(e => e.Comment)
                .IsRequired(false); 
        }); 

        builder.Ignore(p => p.EntityStateAction);
        builder.Ignore(p => p.DomainEvents);
        builder.Ignore(p => p.IsRated); 
    }
}
