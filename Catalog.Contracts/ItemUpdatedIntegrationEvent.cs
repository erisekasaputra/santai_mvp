namespace Catalog.Contracts; 

public record ItemUpdatedIntegrationEvent(
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
    IEnumerable<OwnerReviewIntegrationEvent>? OwnerReviews);