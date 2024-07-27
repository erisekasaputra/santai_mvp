using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;
using Catalog.Domain.Events;
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.ItemAggregate;

public class Item : Entity, IAggregateRoot
{ 
    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal LastPrice { get; private set; }

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } 

    public int StockQuantity { get; private set; }

    public int SoldQuantity { get; private set; }

    public string BrandId { get; private set; }

    public string CategoryId { get; private set; }

    public Brand Brand { get; private set; }

    public Category Category { get; private set; } 

    public DateTime CreatedAt { get; private set; }

    public ICollection<OwnerReview> OwnerReviews { get; set; } 

    public Item()
    {

    }
      
    public Item(string name, string description, decimal price, string imageUrl, DateTime createdAt, int stockQuantity, int soldQuantity, string categoryId, Category category, string brandId, Brand brand, ICollection<OwnerReview> ownerReviews) 
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);
        ArgumentException.ThrowIfNullOrEmpty(imageUrl);
        ArgumentException.ThrowIfNullOrEmpty(categoryId);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentException.ThrowIfNullOrEmpty(brandId);
        ArgumentNullException.ThrowIfNull(brand);
        ArgumentNullException.ThrowIfNull(ownerReviews);
         
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        Category = category;
        BrandId = brandId;
        Brand = brand;
        OwnerReviews = ownerReviews;

        RaiseItemCreatedDomainEvent();
    }

    private void RaiseItemCreatedDomainEvent()
    {
        this.AddDomainEvent(new ItemCreatedDomainEvent(this));
    }

    public void Update(string name, string description, string imageUrl, string categoryId, Category category, string brandId, Brand brand, List<OwnerReview> ownerReviews)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);
        ArgumentException.ThrowIfNullOrEmpty(imageUrl);
        ArgumentException.ThrowIfNullOrEmpty(categoryId);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentException.ThrowIfNullOrEmpty(brandId);
        ArgumentNullException.ThrowIfNull(brand); 

        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        Category = category;
        BrandId = brandId;
        Brand = brand;
        OwnerReviews = ownerReviews;
        this.AddDomainEvent(new ItemUpdatedDomainEvent(this));
    }

    public void Delete()
    {
        this.AddDomainEvent(new ItemDeletedDomainEvent(this.Id));
    }

    public void DecreaseStockQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(StockQuantity, amount, "Stock quantity is not enough");
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1, "Minimum amount for subtracting stock quantity is 1");

        StockQuantity -= amount;
    }

    public void IncreaseStockQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Minimum amount of adding stock quantity is 1");
        StockQuantity += amount;
    }

    public void SetStockQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, "Can not change stock quantity to negative");
        StockQuantity = amount;
    }


    public void DecreaseSoldQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(SoldQuantity, 0, "Can not subtracting sold quantity below than or equal with zero");

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Can not subtracting sold quantity with below than or equal with zero");

        if (SoldQuantity - amount < 0)
        {
            SoldQuantity = 0;
            return;
        }

        SoldQuantity -= amount;
    }

    public void IncreaseSoldQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Minimum amount of adding sold quantity is 1");
        SoldQuantity += amount;
    }

    public void SetSoldQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, "Can not change sold quantity to less than zero.");
        SoldQuantity = amount;
    }

    public void SetPrice(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Can not set price to less than or equal with zero.");

        LastPrice = Price;
        Price = amount;
    }
}
