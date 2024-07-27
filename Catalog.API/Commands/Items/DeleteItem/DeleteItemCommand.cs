using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Items.DeleteItem;

public record DeleteItemCommand(string Id) : IRequest<Result<Unit>>;