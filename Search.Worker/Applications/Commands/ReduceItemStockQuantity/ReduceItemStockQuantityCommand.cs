using MediatR;

namespace Search.Worker.Applications.Commands.ReduceItemStockQuantity;

internal record ReduceItemStockQuantityCommand(Guid Id, int Quantity) : IRequest<Unit>;
