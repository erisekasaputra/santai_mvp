using MediatR;

namespace Search.Worker.Applications.Commands.SetItemStockQuantity;

internal record SetItemStockQuantityCommand(string Id, int Quantity) : IRequest<Unit>; 
