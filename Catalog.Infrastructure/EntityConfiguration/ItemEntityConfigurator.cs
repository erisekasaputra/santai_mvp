using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Catalog.Domain.Aggregates.ItemAggregate;

namespace Catalog.Infrastructure.EntityConfiguration; 
public class ItemEntityConfigurator : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> catalogItem)
    {
        catalogItem.ToTable("items");

        catalogItem.HasKey(x => x.Id);

        catalogItem.Property(x => x.Id)
            .HasMaxLength(26);

        catalogItem.Property(b => b.Name)
            .HasMaxLength(50);

        catalogItem.Property(b => b.Description)
            .HasMaxLength(1000);
        
        catalogItem.Property(b => b.ImageUrl)
            .HasMaxLength(500);
         
        catalogItem.HasOne(ci => ci.Category)
            .WithMany(ic => ic.Items)
            .HasForeignKey(ci => ci.CategoryId);

        catalogItem.Property(ci => ci.LastPrice).HasColumnType("decimal(18, 2)");

        catalogItem.Property(ci => ci.Price).HasColumnType("decimal(18, 2)");
    }
}
