using MediatR;

namespace Catalog.Contracts;

public record ItemCreatedIntegrationEvent(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    DateTime CreatedAt,
    int StockQuantity,
    int SoldQuantity,
    string? CategoryId,
    string? CategoryName,
    string? CategoryImageUrl,
    string? BrandId,
    string? BrandName,
    string? BrandImageUrl,
    bool IsActive,
    bool IsDeleted,
    IEnumerable<OwnerReviewIntegrationEvent>? OwnerReviews) : INotification;