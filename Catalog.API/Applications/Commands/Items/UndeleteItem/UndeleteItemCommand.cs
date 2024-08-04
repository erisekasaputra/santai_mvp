using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.UndeleteItem;

public record UndeleteItemCommand(string Id) : IRequest<Result<Unit>>;