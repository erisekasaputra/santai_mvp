using Catalog.Domain.Aggregates.OwnerReviewAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntityConfiguration;

public class OwnerReviewEntityConfigurator : IEntityTypeConfiguration<OwnerReview>
{
    public void Configure(EntityTypeBuilder<OwnerReview> builder)
    {
        builder.ToTable("owner_reviews");
         
        builder.HasKey(or => or.Id);

        builder.Property(or => or.Id)
            .HasMaxLength(26);

        builder.Property(or => or.Title)
            .HasMaxLength(25)
            .IsRequired(); 
    }  
}
