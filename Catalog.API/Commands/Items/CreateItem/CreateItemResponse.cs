using Catalog.Domain.Aggregates.OwnerReviewAggregate;

namespace Catalog.API.Commands.Items.CreateItem;

public record CreateItemResponse(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    DateTime CreatedAt,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId,
    string CategoryName,
    string BrandId,
    string BrandName,
    IEnumerable<OwnerReview> OwnerReviews);