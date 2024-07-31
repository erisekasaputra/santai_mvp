using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeleteItem;

public record DeleteItemCommand(string Id) : IRequest<Result<Unit>>;