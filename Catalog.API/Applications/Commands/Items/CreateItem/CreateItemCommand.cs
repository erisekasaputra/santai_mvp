using Catalog.API.DTOs.Item;
using Catalog.API.DTOs.OwnerReview;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.CreateItem;

public record CreateItemCommand(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId,
    string BrandId,
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result<ItemDto>>;
