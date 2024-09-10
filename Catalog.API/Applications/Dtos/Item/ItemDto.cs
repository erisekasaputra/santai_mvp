using Catalog.API.Applications.Dtos.OwnerReview;
using Core.Enumerations;
namespace Catalog.API.Applications.Dtos.Item;

public record ItemDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    Currency Currency,
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