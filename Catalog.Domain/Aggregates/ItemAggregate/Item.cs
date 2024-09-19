using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.Aggregates.OwnerReviewAggregate; 
using Catalog.Domain.Events; 
using Catalog.Domain.SeedWork;
using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;

namespace Catalog.Domain.Aggregates.ItemAggregate;

public class Item : Entity, IAggregateRoot
{ 
    public string Name { get; private set; } 
    public string Description { get; private set; }  
    public string Sku { get; private set; } 
    public string ImageUrl { get; private set; }  
    public int StockQuantity { get; private set; }  
    public int SoldQuantity { get; private set; } 
    public Guid? BrandId { get; set; }  
    public Guid? CategoryId { get; private set; }  
    public Brand? Brand { get; private set; } 
    public Category? Category { get; private set; }  
    public DateTime CreatedAt { get; private set; }
    public decimal LastPrice { get; private set; }  
    public bool IsActive { get; private set; }  
    public bool IsDeleted { get; private set; } 
    public Money Price { get; private set; }  
    public ICollection<OwnerReview> OwnerReviews { get; private set; }

    public Item()
    {
        Sku = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        ImageUrl = string.Empty;
        OwnerReviews = [];
        Price = null!;
    }
      
    public Item(
        string name,
        string description,
        decimal price,
        Currency currency,
        string sku,
        string imageUrl,
        DateTime createdAt,
        int stockQuantity,
        int soldQuantity,
        Guid categoryId,
        Category category,
        Guid brandId,
        Brand brand,
        bool isActive,
        ICollection<OwnerReview> ownerReviews) : this()
    {
        OwnerReviews ??= [];

        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);
        ArgumentException.ThrowIfNullOrEmpty(imageUrl); 
        ArgumentNullException.ThrowIfNull(category); 
        ArgumentNullException.ThrowIfNull(brand); 
        ArgumentNullException.ThrowIfNull(ownerReviews);

        if (ownerReviews.Count == 0) throw new DomainException("Owner review can not be empty");
         
        Name = name;
        Description = description;
        Price = new Money(price, currency);
        Sku = sku;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        Category = category;
        BrandId = brandId;
        Brand = brand;
        IsActive = isActive;
        IsDeleted = false;

        foreach (var ownerReview in ownerReviews) 
        {
            var reviewExists = OwnerReviews.Where(x => x.Equals(ownerReview)).Any();
            if (!reviewExists)
            {
                OwnerReviews.Add(ownerReview);
            }
        }

        RaiseItemCreatedDomainEvent(this);
    }
    public void Update(
        string name,
        string description,
        string sku,
        string imageUrl,
        Guid categoryId,
        Category category,
        Guid brandId,
        Brand brand,
        bool isActive,
        ICollection<OwnerReview> ownerReviews,
        decimal price,
        Currency currency,
        int stockQuantity,
        int soldQuantity)
    {
        OwnerReviews ??= [];

        if (IsDeleted)
        {
            throw new DomainException("Can not update data when the data was deleted");
        }
         
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);
        ArgumentException.ThrowIfNullOrEmpty(sku);
        ArgumentException.ThrowIfNullOrEmpty(imageUrl); 
        ArgumentNullException.ThrowIfNull(category); 
        ArgumentNullException.ThrowIfNull(brand);
        ArgumentNullException.ThrowIfNull(ownerReviews);

        if (ownerReviews.Count == 0) throw new DomainException("Owner review can not be empty");

        Name = name;
        Description = description;
        Sku = sku;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        Category = category;
        BrandId = brandId;
        Brand = brand;
        IsActive = isActive;

        LastPrice = Price.Amount; 
        if (price != Price.Amount)
        {
            Price.SetAmount(price, currency);
        }

        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;

        OwnerReviews ??= []; 
        OwnerReviews.Clear(); 

        foreach (var ownerReview in ownerReviews)
        {
            var reviewExists = OwnerReviews.Where(x => x.Equals(ownerReview)).Any();
            if (!reviewExists)
            {
                OwnerReviews.Add(ownerReview);
            }
        }   

        RaiseItemUpdatedDomainEvent(this);
    }

    public void SetActive()
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not edit data for a deleted item");
        }

        IsActive = true;
        RaiseItemActivatedDomainEvent(Id);
    } 

    public void SetInactive()
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not edit data for a deleted item");
        }

        IsActive = false;
        RaiseItemInactivatedDomainEvent(Id);
    } 
   
    public void SetDelete()
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not delete data if the data is already deleted");
        }

        IsActive = false;
        IsDeleted = true; 
        RaiseItemDeletedDomainEvent(Id);
    }

    public void SetUndeleted()
    {
        if (!IsDeleted)
        {
            throw new DomainException("Can not set data to undelete when the data is already undeleted");
        }
        IsActive = false;
        IsDeleted = false;
        RaiseItemUndeletedDomainEvent(Id);
    } 

    public void ReduceStockQuantity(int amount)
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not reduce stock to a deleted item");
        } 

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
        if (IsDeleted)
        {
            throw new DomainException("Can not set stock quantity from a deleted item");
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, "Can not change stock quantity to negative");
        StockQuantity = amount;

        RaiseItemStockSetDomainEvent(Id, amount);
    } 

    public void ReduceSoldQuantity(int amount)
    {  
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(SoldQuantity, 0, "Can not subtracting sold quantity below than or equal with zero");

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Can not subtracting sold quantity with below than or equal with zero");

        if ((SoldQuantity - amount) < 0)
        {
            throw new DomainException("Can not reduce sold quantity into negative number");
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
        if (IsDeleted)
        {
            throw new DomainException("Can not set sold quantity from a deleted item");
        }  

        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, "Can not change sold quantity to less than zero.");
        
        SoldQuantity = amount;

        RaiseItemSoldSetDomainEvent(Id, amount);
    } 
    
    public void SetPrice(decimal amount, Currency currency)
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not set item price from a deleted item");
        } 

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0, "Can not set price to less than or equal with zero.");

        LastPrice = Price.Amount;
        Price.SetAmount(amount, currency);

        RaiseItemPriceSetDomainEvent(Id, LastPrice, amount, currency);
    }

    private void RaiseItemPriceSetDomainEvent(Guid id, decimal oldPrice, decimal newPrice, Currency currency)
    {
        AddDomainEvent(new ItemPriceSetDomainEvent(id, oldPrice, newPrice, currency));
    }

    private void RaiseItemSoldSetDomainEvent(Guid id, int quantity)
    {
        AddDomainEvent(new ItemSoldSetDomainEvent(id, quantity));
    }

    private void RaiseItemSoldAddedDomainEvent(Guid id, int quantity)
    {
        AddDomainEvent(new ItemSoldAddedDomainEvent(id, quantity));
    }

    private void RaiseItemSoldReducedDomainEvent(Guid id, int quantity)
    {
        AddDomainEvent(new ItemSoldReducedDomainEvent(id, quantity));
    }

    private void RaiseItemStockSetDomainEvent(Guid id, int quantity)
    {
        AddDomainEvent(new ItemStockSetDomainEvent(id, quantity));
    }

    private void RaiseItemStockAddedDomainEvent(Guid id, int quantity)
    {
        AddDomainEvent(new ItemStockAddedDomainEvent(id, quantity));
    }

    private void RaiseItemStockReducedDomainEvent(Guid id, int quantity)
    {
        AddDomainEvent(new ItemStockReducedDomainEvent(id, quantity));
    }

    private void RaiseItemCreatedDomainEvent(Item item)
    {
        AddDomainEvent(new ItemCreatedDomainEvent(item));
    }
    private void RaiseItemDeletedDomainEvent(Guid id)
    {
        AddDomainEvent(new ItemDeletedDomainEvent(id));
    }

    private void RaiseItemUpdatedDomainEvent(Item item)
    { 
        AddDomainEvent(new ItemUpdatedDomainEvent(item));
    } 
    private void RaiseItemInactivatedDomainEvent(Guid id)
    {
        AddDomainEvent(new ItemInactivatedDomainEvent(id));
    }
    private void RaiseItemUndeletedDomainEvent(Guid id)
    {
        AddDomainEvent(new ItemUndeletedDomainEvent(id));
    }
    private void RaiseItemActivatedDomainEvent(Guid id)
    {
        AddDomainEvent(new ItemActivatedDomainEvent(id));
    }
}
