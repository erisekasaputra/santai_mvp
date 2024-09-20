
using Core.Enumerations;
using Core.Events.Catalog;
using MediatR;

namespace Search.Worker.Applications.Commands.UpdateItem;

public record UpdateItemCommand(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal OldPrice,
    decimal NewPrice,
    Currency Currency,
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
    IEnumerable<OwnerReviewIntegrationEvent> OwnerReviews) : IRequest<Unit>;