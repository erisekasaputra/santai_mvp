
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.ItemAggregate;

public class Item : Entity<string>, IAggregateRoot
{
    public string CategoryId { get; set; }

    public Category Category { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal LastPrice { get; private set; }

    public decimal Price { get; private set; }

    public string ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public int StockQuantity { get; private set; }

    public int SoldQuantity { get; private set; }
    
    public Item(string id, string name, string description, decimal price, string imageUrl, DateTime createdAt, int stockQuantity, int soldQuantity, string categoryId, Category category) 
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        Category = category;
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
