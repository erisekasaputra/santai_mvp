using Catalog.Contracts;
using MediatR;

namespace Search.Worker.Applications.Commands.CreateItem;

public record CreateItemCommand (
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string ImageUrl, 
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
    IEnumerable<OwnerReviewIntegrationEvent> OwnerReviews) : IRequest<Unit>;