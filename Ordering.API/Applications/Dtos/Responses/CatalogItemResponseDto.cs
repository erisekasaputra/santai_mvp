using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class CatalogItemResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
    public Currency? Currency { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? StockQuantity { get; set; }
    public int? SoldQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; } 
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public bool? IsActive { get; set; }
    public IEnumerable<CatalogItemOwnerReviewResponseDto>? OwnerReviews { get; set; }

    public CatalogItemResponseDto(
        Guid id,
        string? name,
        string? description,
        string? sku,
        decimal? price,
        Currency? currency,
        string? imageUrl,
        DateTime createdAt,
        int? stockQuantity,
        int? soldQuantity,
        Guid? categoryId,
        string? categoryName,
        Guid? brandId,
        string? brandName,
        bool? isActive,
        IEnumerable<CatalogItemOwnerReviewResponseDto>? ownerReviews)
    {
        Id = id;
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        Currency = currency;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        StockQuantity = stockQuantity;
        SoldQuantity = soldQuantity;
        CategoryId = categoryId;
        CategoryName = categoryName;
        BrandName = brandName;
        BrandId = brandId;
        IsActive = isActive;
        OwnerReviews = ownerReviews;
    }
}
