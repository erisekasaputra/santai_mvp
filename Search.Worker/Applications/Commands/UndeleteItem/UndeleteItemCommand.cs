using MediatR;

namespace Search.Worker.Applications.Commands.UndeleteItem;

internal record UndeleteItemCommand(string Id) : IRequest<Unit>;
