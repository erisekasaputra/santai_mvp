using MediatR;

namespace Search.Worker.Applications.Commands.ReduceSold;

internal record ReduceSoldCommand(string Id, int Quantity) : IRequest<Unit>;