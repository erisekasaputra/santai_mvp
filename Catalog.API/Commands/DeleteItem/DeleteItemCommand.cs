using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.DeleteItem;

public record DeleteItemCommand(string Id) : IRequest<Result<Unit>>
{
}
