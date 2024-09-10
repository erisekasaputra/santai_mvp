
using Catalog.API.Applications.Dtos.OwnerReview;
using Core.Enumerations;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.UpdateItem;

public record UpdateItemCommand(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    Currency Currency,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    Guid CategoryId,
    Guid BrandId,
    bool IsActive,
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result>;
