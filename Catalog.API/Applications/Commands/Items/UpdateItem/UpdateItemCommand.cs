using Catalog.API.DTOs.OwnerReview;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.UpdateItem;

public record UpdateItemCommand(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId,
    string BrandId,
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result<Unit>>;
