using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Catalog.Domain.Aggregates.ItemAggregate;
using Core.Enumerations; 
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Core.Utilities;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Catalog.Infrastructure.EntityConfiguration; 
public class ItemEntityConfigurator : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    { 
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(26); 

        builder.Property(b => b.Name)
            .HasMaxLength(50);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.Sku)
           .HasMaxLength(50);

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

        var converter = new ValueConverter<ICollection<OwnerReview>, string>(
             v => CustomSerializer.Serialize(v),
             v => CustomSerializer.Deserialize<List<OwnerReview>>(v)  
        );

        var comparer = new ValueComparer<ICollection<OwnerReview>>(
             (c1, c2) =>
                 (c1 != null && c2 != null && c1.SequenceEqual(c2)), 
             c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Title.GetHashCode(), v.Rating.GetHashCode())), 
             c => c == null ? new List<OwnerReview>() : c.Select(v => new OwnerReview(v.Title, v.Rating)).ToList() 
        );

        builder.Property(i => i.OwnerReviews)
            .HasConversion(converter)
            .IsUnicode(false)
            .Metadata.SetValueComparer(comparer); 

        builder.Property(ci => ci.LastPrice)
            .HasColumnType("decimal(18, 2)");

        builder.OwnsOne(p => p.Price, buildAction =>
        { 
            buildAction.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(18, 4);

            buildAction.Property(e => e.Currency)
                .HasConversion(
                    val => val.ToString(),
                    val => Enum.Parse<Currency>(val))
                .IsRequired();
        }); 
        
        builder.Ignore(x => x.DomainEvents);
    }
}
