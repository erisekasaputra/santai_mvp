using MediatR;

namespace Search.Worker.Applications.Commands.InactivateItem;

internal record InactivateItemCommand(string Id) : IRequest<Unit>; 
