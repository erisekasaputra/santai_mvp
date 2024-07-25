using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.UpdateItem;

public record UpdateItemCommand(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId) : IRequest<Result<Unit>>
{
}
