using MediatR;

namespace Search.Worker.Applications.Commands.ActivateItem;

internal record ActivateItemCommand(string Id) : IRequest<Unit>;
