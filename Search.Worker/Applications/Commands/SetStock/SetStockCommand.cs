using MediatR;

namespace Search.Worker.Applications.Commands.SetStock;

internal record SetStockCommand(string Id, int Quantity) : IRequest<Unit>; 
