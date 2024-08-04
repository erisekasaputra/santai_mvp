namespace Search.Worker.Domain.Models;

public class Item
{
    public string Id { get; init; }
    public string Name { get; private set; }
    public string Description { get; private  set; }
    public decimal Price { get; private set; }
    public string ImageUrl { get; private set; }
    public DateTime CreatedAt { get; init; }
    public int StockQuantity { get; private set; }
    public int SoldQuantity { get; private set; }
    public string? CategoryId { get; private set; }
    public string? CategoryName { get; private set; }
    public string? CategoryImageUrl { get; private set; }
    public string? BrandId { get; private set; }
    public string? BrandName { get; private set; }
    public string? BrandImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public IEnumerable<OwnerReview>? OwnerReviews { get; private set; }

    public Item(string id,
                string name,
                string description,
                decimal price,
                string imageUrl,
                DateTime createdAt,
                int stockQuantity,
                int soldQuantity,
                string? categoryId,
                string? categoryName,
                string? categoryImageUrl,
                string? brandId,
                string? brandName,
                string? brandImageUrl,
                bool isActive,
                bool isDeleted,
                IEnumerable<OwnerReview>? ownerReviews)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        CategoryName = categoryName;
        CategoryImageUrl = categoryImageUrl;
        BrandId = brandId;
        BrandName = brandName;
        BrandImageUrl = brandImageUrl;
        IsActive = isActive;
        IsDeleted = isDeleted;
        OwnerReviews = ownerReviews;
    }
    public void Update( string name,
                        string description,
                        decimal price,
                        string imageUrl, 
                        int stockQuantity,
                        int soldQuantity,
                        string? categoryId,
                        string? categoryName,
                        string? categoryImageUrl,
                        string? brandId,
                        string? brandName,
                        string? brandImageUrl,
                        bool isActive,
                        bool isDeleted,
                        IEnumerable<OwnerReview>? ownerReviews)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        CategoryName = categoryName;
        CategoryImageUrl = categoryImageUrl;
        BrandId = brandId;
        BrandName = brandName;
        BrandImageUrl = brandImageUrl; 
        IsActive = isActive;
        IsDeleted = isDeleted;
        OwnerReviews = ownerReviews;
    }

    internal void Activate()
    {
        IsActive = true;
    }

    internal void AddSoldQuantity(int quantity)
    {
        SoldQuantity += quantity;
    }

    internal void AddStockQuantity(int quantity)
    {
        StockQuantity += quantity;
    }

    internal void Delete()
    {
        IsActive = false;
        IsDeleted = true;
    }

    internal void Deactivate()
    {
        IsActive = false;
    }

    internal void ReduceSoldQuantity(int quantity)
    {
        SoldQuantity -= quantity;
    }

    internal void ReduceStockQuantity(int quantity)
    {
        StockQuantity -= quantity;
    }

    internal void SetItemPrice(decimal price)
    {
        Price = price;
    }

    internal void SetSoldQuantity(int quantity)
    {
        SoldQuantity = quantity;
    }

    internal void SetStockQuantity(int quantity)
    {
        StockQuantity = quantity;
    }

    internal void Undelete()
    {
        IsActive = false;
        IsDeleted = false;
    }
} 