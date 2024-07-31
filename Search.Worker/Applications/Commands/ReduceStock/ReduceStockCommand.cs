using MediatR;

namespace Search.Worker.Applications.Commands.ReduceStock;

internal record ReduceStockCommand(string Id, int Quantity) : IRequest<Unit>;
