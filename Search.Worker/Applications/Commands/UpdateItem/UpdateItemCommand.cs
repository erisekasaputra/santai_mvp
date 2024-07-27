using MediatR;

namespace Search.Worker.Applications.Commands.UpdateItem;

public record UpdateItemCommand(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    DateTime CreatedAt,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId,
    string CategoryName,
    string BrandId,
    string BrandName) : IRequest<Unit>;