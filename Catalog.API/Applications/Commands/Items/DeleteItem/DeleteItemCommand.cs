using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeleteItem;

public record DeleteItemCommand(Guid Id) : IRequest<Result<Unit>>;