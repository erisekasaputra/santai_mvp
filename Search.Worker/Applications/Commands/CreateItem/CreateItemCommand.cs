using Catalog.Contracts;
using MediatR;

namespace Search.Worker.Applications.Commands.CreateItem;

public record CreateItemCommand (
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
    IEnumerable<OwnerReviewIntegrationEvent> OwnerReviews) : IRequest<Unit>;