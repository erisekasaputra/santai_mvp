using MediatR;

namespace Search.Worker.Applications.Commands.InactivateItem;

internal record InactivateItemCommand(Guid Id) : IRequest<Unit>; 
