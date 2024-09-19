namespace Search.API.Domain.Models;

public class Item
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal LastPrice { get; set; }
    public decimal Price { get; set; }
    public required string ImageUrl { get; set; }
    public DateTime CreatedAt { get; init; }
    public int StockQuantity { get; set; }
    public int SoldQuantity { get; set; }
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryImageUrl { get; set; }
    public string? BrandId { get; set; }
    public string? BrandName { get; set; }
    public string? BrandImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public IEnumerable<OwnerReview>? OwnerReviews { get; set; }
}