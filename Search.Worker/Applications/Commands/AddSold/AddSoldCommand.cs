using MediatR;

namespace Search.Worker.Applications.Commands.AddSold;

internal record AddSoldCommand(string Id, int Quantity) : IRequest<Unit>;
