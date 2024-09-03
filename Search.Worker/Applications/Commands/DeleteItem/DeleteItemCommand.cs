using MediatR;

namespace Search.Worker.Applications.Commands.DeleteItem;

public record DeleteItemCommand(Guid Id) : IRequest<Unit>;
