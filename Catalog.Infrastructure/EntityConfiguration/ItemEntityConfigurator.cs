using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Catalog.Domain.Aggregates.ItemAggregate;
using Newtonsoft.Json;
using Catalog.Domain.Aggregates.BrandAggregate;

namespace Catalog.Infrastructure.EntityConfiguration; 
public class ItemEntityConfigurator : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(26); 

        builder.Property(b => b.Name)
            .HasMaxLength(50);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.ImageUrl)
            .HasMaxLength(500); 

        builder.Property(ci => ci.CategoryId)
           .IsRequired(false);

        builder.Property(ci => ci.BrandId)
            .IsRequired(false);  
         
        builder.HasOne(ci => ci.Category)
            .WithMany(ic => ic.Items)
            .HasForeignKey(ci => ci.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);  

        builder.HasOne(ci => ci.Brand)
            .WithMany(ic => ic.Items)
            .HasForeignKey(ci => ci.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.OwnerReviews)
            .WithOne()
            .HasForeignKey("ItemId")
            .OnDelete(DeleteBehavior.Cascade);  

        builder.Property(ci => ci.LastPrice).HasColumnType("decimal(18, 2)");

        builder.Property(ci => ci.Price).HasColumnType("decimal(18, 2)");
        
        builder.Ignore(x => x.DomainEvents);
    }
}
