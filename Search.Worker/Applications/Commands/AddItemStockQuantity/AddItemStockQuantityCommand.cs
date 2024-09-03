using MediatR;

namespace Search.Worker.Applications.Commands.AddItemStockQuantity;

internal record AddItemStockQuantityCommand(Guid Id, int Quantity) : IRequest<Unit>;
