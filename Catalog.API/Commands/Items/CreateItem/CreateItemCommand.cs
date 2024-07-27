using Catalog.API.DTOs.OwnerReviewDto;
using Catalog.API.SeedWorks; 
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
    IEnumerable<OwnerReviewDto> OwnerReviews) : IRequest<Result<CreateItemResponse>>;
