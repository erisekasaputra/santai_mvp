using MediatR;

namespace Search.Worker.Applications.Commands.AddStock;

internal record AddStockCommand(string Id, int Quantity) : IRequest<Unit>;
