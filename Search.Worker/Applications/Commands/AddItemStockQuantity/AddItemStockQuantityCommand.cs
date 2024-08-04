using MediatR;

namespace Search.Worker.Applications.Commands.AddItemStockQuantity;

internal record AddItemStockQuantityCommand(string Id, int Quantity) : IRequest<Unit>;
