using MediatR;

namespace Catalog.Contracts; 

public record ItemUpdatedIntegrationEvent(
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
    string? CategoryName,
    string? CategoryImageUrl,
    Guid? BrandId,
    string? BrandName,
    string? BrandImageUrl,
    bool IsActive,
    bool IsDeleted,
    IEnumerable<OwnerReviewIntegrationEvent>? OwnerReviews) : INotification;