using Catalog.API.SeedWorks;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;
using MediatR;

namespace Catalog.API.Commands.Items.UpdateItem;

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
    IEnumerable<OwnerReview> OwnerReviews) : IRequest<Result<Unit>>;
