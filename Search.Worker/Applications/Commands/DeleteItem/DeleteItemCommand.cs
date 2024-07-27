using MediatR;

namespace Search.Worker.Applications.Commands.DeleteItem;

public record DeleteItemCommand (
    string Id) : IRequest<Unit>
{
}
