using Catalog.API.SeedWorks;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;
using MediatR;

namespace Catalog.API.Commands.Items.CreateItem; 

public record CreateItemCommand(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId,
    string BrandId,
    IEnumerable<OwnerReview> OwnerReviews) : IRequest<Result<CreateItemResponse>>;
