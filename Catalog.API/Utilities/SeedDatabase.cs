using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;
using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Utilities; 

public class SeedDatabase
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    { 
        var dbContext = serviceProvider.GetRequiredService<CatalogDbContext>(); 
        try
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(); 

            var category = new Category("Oil", "5d14bfb1-8ca3-49c1-b3ef-1f7b8788389b.png");
            var brand = new Brand("Yamalube", "5d14bfb1-8ca3-49c1-b3ef-1f228788389b.jpg");

            var item = new Item(
                "Oil Yamaha Delta",
                "Oil with very good performance",
                100,
                Core.Enumerations.Currency.MYR,
                "SKU123",
                "5d14bfb1-8ca3-49c1-b3ef-1f228788389b.jpg",
                DateTime.UtcNow,
                100,
                0,
                category.Id,
                category,
                brand.Id,
                brand,
                true,
                [new OwnerReview("Performance", 9)]);

            await dbContext.Brands.AddAsync(brand);
            await dbContext.Categories.AddAsync(category);
            await dbContext.Items.AddAsync(item);

            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

