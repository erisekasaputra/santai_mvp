using Catalog.API.DTOs.OwnerReview;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.UpdateItem;

public record UpdateItemCommand(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    Guid CategoryId,
    Guid BrandId,
    bool IsActive,
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result<Unit>>;
