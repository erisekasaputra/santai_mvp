using Catalog.API.DTOs.Item;
using Catalog.API.DTOs.OwnerReview;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.CreateItem;

public record CreateItemCommand(
    string Name,
    string Description,
    decimal Price,
    string Sku,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    Guid CategoryId,
    Guid BrandId,
    bool IsActive,
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result<ItemDto>>;
