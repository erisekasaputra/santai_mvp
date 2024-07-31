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


    private readonly IList<OwnerReview> _ownerReviews;

    public IReadOnlyCollection<OwnerReview> OwnerReviews => _ownerReviews.AsReadOnly(); 

    public Item()
    {
        _ownerReviews = [];
    }
      
    public Item(string name, string description, decimal price, string imageUrl, DateTime createdAt, int stockQuantity, int soldQuantity, string categoryId, Category category, string brandId, Brand brand, ICollection<OwnerReview> ownerReviews) : this()
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
        Price = price;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        Category = category;
        BrandId = brandId;
        Brand = brand;
         
        if (ownerReviews is not null)
        {
            var filteredOwnerReview = ownerReviews.GroupBy(r => r.Title).Select(g => g.First()).ToList();
            foreach (var ownerReview in filteredOwnerReview)
            {
                _ownerReviews.Add(ownerReview);
            } 
        }

        RaiseItemCreatedDomainEvent(this);
    } 
      

    public void Update(string name, string description, string imageUrl, string categoryId, Category category, string brandId, Brand brand, ICollection<OwnerReview> ownerReviews)
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

        _ownerReviews.Clear();
        if (ownerReviews is not null)
        {
            var filteredOwnerReview = ownerReviews.GroupBy(r => r.Title).Select(g => g.First()).ToList();
            foreach (var ownerReview in filteredOwnerReview)
            {
                _ownerReviews.Add(ownerReview);
            }   
        }

        RaiseItemUpdatedDomainEvent(this);
    }

    public void Delete()
    {
        RaiseItemDeletedDomainEvent(Id);
    }

    public void ReduceStockQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(StockQuantity, amount, "Stock quantity is insufficient");
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1, "Minimum amount for subtracting stock quantity is 1");

        StockQuantity -= amount;

        RaiseItemStockReducedDomainEvent(Id, amount);
    }

    

    public void AddStockQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Minimum amount of adding stock quantity is 1");
        StockQuantity += amount;

        RaiseItemStockAddedDomainEvent(Id, amount);
    } 

    public void SetStockQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, "Can not change stock quantity to negative");
        StockQuantity = amount;

        RaiseItemStockSetDomainEvent(Id, amount);
    } 

    public void ReduceSoldQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(SoldQuantity, 0, "Can not subtracting sold quantity below than or equal with zero");

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Can not subtracting sold quantity with below than or equal with zero");

        if (SoldQuantity - amount < 0)
        {
            SoldQuantity = 0;
            return;
        }

        SoldQuantity -= amount;

        RaiseItemSoldReducedDomainEvent(Id, amount);
    } 

    public void AddSoldQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Minimum amount of adding sold quantity is 1");
        SoldQuantity += amount;

        RaiseItemSoldAddedDomainEvent(Id, amount);
    } 
   
    public void SetSoldQuantity(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, "Can not change sold quantity to less than zero.");
        SoldQuantity = amount;

        RaiseItemSoldSetDomainEvent(Id, amount);
    } 
    
    public void SetPrice(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Can not set price to less than or equal with zero.");

        LastPrice = Price;
        Price = amount;

        RaiseItemPriceSetDomainEvent(Id, amount);
    }

    private void RaiseItemPriceSetDomainEvent(string id, decimal price)
    {
        AddDomainEvent(new ItemPriceSetDomainEvent(id, price));
    }

    private void RaiseItemSoldSetDomainEvent(string id, int quantity)
    {
        AddDomainEvent(new ItemSoldSetDomainEvent(id, quantity));
    }

    private void RaiseItemSoldAddedDomainEvent(string id, int quantity)
    {
        AddDomainEvent(new ItemSoldAddedDomainEvent(id, quantity));
    }

    private void RaiseItemSoldReducedDomainEvent(string id, int quantity)
    {
        AddDomainEvent(new ItemSoldReducedDomainEvent(id, quantity));
    }

    private void RaiseItemStockSetDomainEvent(string id, int quantity)
    {
        AddDomainEvent(new ItemStockSetDomainEvent(id, quantity));
    }

    private void RaiseItemStockAddedDomainEvent(string id, int quantity)
    {
        AddDomainEvent(new ItemStockAddedDomainEvent(id, quantity));
    }

    private void RaiseItemStockReducedDomainEvent(string id, int quantity)
    {
        AddDomainEvent(new ItemStockReducedDomainEvent(id, quantity));
    }

    private void RaiseItemCreatedDomainEvent(Item item)
    {
        AddDomainEvent(new ItemCreatedDomainEvent(item));
    }
    private void RaiseItemDeletedDomainEvent(string id)
    {
        AddDomainEvent(new ItemDeletedDomainEvent(id));
    }

    private void RaiseItemUpdatedDomainEvent(Item item)
    { 
        AddDomainEvent(new ItemUpdatedDomainEvent(item));
    }
}
