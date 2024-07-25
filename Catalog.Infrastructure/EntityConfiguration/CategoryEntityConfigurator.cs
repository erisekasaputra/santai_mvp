using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Catalog.Domain.Aggregates.CategoryAggregate;

namespace Catalog.Infrastructure.EntityConfiguration;

public class CategoryEntityConfigurator : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(26);

        builder.Property(x => x.Name) 
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(500); 
    }
}