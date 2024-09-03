using MediatR;

namespace Search.Worker.Applications.Commands.ActivateItem;

internal record ActivateItemCommand(Guid Id) : IRequest<Unit>;
