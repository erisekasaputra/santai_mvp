namespace Search.Worker.Domain.Models;

public class Item(string Id,
    string name,
    string description,
    decimal price,
    string imageUrl,
    DateTime createdAt,
    int stockQuantity,
    int soldQuantity,
    string categoryId,
    string categoryName,
    string brandId,
    string brandName,
    IEnumerable<OwnerReview>? ownerReviews)
{
    public string Id { get; init; } = Id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    public string ImageUrl { get; set; } = imageUrl;
    public DateTime CreatedAt { get; init; } = createdAt;
    public int StockQuantity { get; set; } = stockQuantity;
    public int SoldQuantity { get; set; } = soldQuantity;
    public string CategoryId { get; set; } = categoryId;
    public string CategoryName { get; set; } = categoryName;
    public string BrandId { get; set; } = brandId;
    public string BrandName { get; set; } = brandName;
    public IEnumerable<OwnerReview>? OwnerReviews { get; } = ownerReviews;
} 