using Catalog.API.DTOs.OwnerReview; 

namespace Catalog.API.DTOs.Item;

public record ItemDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string ImageUrl,
    DateTime CreatedAt,
    int StockQuantity,
    int SoldQuantity,
    Guid? CategoryId,
    string CategoryName,
    Guid? BrandId,
    string BrandName,
    bool IsActive,
    IEnumerable<OwnerReviewDto>? OwnerReviews);