namespace Search.Worker.Domain.Models;

public class Item(
    Guid id,
    string name,
    string description,
    string sku,
    decimal price,
    string imageUrl,
    int stockQuantity,
    int soldQuantity,
    Guid? categoryId,
    string? categoryName,
    string? categoryImageUrl,
    Guid? brandId,
    string? brandName,
    string? brandImageUrl,
    bool isActive,
    bool isDeleted,
    IEnumerable<OwnerReview>? ownerReviews)
{
    public Guid Id { get; init; } = id;
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public string Sku { get; private set; } = sku;
    public decimal Price { get; private set; } = price;
    public string ImageUrl { get; private set; } = imageUrl;
    public int StockQuantity { get; private set; } = stockQuantity;
    public int SoldQuantity { get; private set; } = soldQuantity;
    public Guid? CategoryId { get; private set; } = categoryId;
    public string? CategoryName { get; private set; } = categoryName;
    public string? CategoryImageUrl { get; private set; } = categoryImageUrl;
    public Guid? BrandId { get; private set; } = brandId;
    public string? BrandName { get; private set; } = brandName;
    public string? BrandImageUrl { get; private set; } = brandImageUrl;
    public bool IsActive { get; private set; } = isActive;
    public bool IsDeleted { get; private set; } = isDeleted;
    public IEnumerable<OwnerReview>? OwnerReviews { get; private set; } = ownerReviews;

    public void Update(
        string name,
        string description,
        string sku,
        decimal price,
        string imageUrl,
        int stockQuantity,
        int soldQuantity,
        Guid? categoryId,
        string? categoryName,
        string? categoryImageUrl,
        Guid? brandId,
        string? brandName,
        string? brandImageUrl,
        bool isActive,
        bool isDeleted,
        IEnumerable<OwnerReview>? ownerReviews)
    {
        Name = name;
        Description = description;
        Sku = sku;
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