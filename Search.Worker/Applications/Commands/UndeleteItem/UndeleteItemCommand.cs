using MediatR;

namespace Search.Worker.Applications.Commands.UndeleteItem;

internal record UndeleteItemCommand(Guid Id) : IRequest<Unit>;
