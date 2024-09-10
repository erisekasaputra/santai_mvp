using Catalog.API.Applications.Dtos.OwnerReview;
using Core.Enumerations;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.CreateItem;

public record CreateItemCommand(
    string Name,
    string Description,
    decimal Price,
    Currency Currency,
    string Sku,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    Guid CategoryId,
    Guid BrandId,
    bool IsActive,
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result>;
