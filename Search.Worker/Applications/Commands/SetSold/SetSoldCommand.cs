using MediatR;

namespace Search.Worker.Applications.Commands.SetSold;

internal record SetSoldCommand(string Id, int Quantity) : IRequest<Unit>;