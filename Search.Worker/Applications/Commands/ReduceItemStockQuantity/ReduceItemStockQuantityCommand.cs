using MediatR;

namespace Search.Worker.Applications.Commands.ReduceItemStockQuantity;

internal record ReduceItemStockQuantityCommand(string Id, int Quantity) : IRequest<Unit>;
