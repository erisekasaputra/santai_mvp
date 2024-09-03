using MediatR;

namespace Search.Worker.Applications.Commands.SetItemStockQuantity;

internal record SetItemStockQuantityCommand(Guid Id, int Quantity) : IRequest<Unit>; 
