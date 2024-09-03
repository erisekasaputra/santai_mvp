using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ActivateItem;

public record ActivateItemCommand(Guid Id) : IRequest<Result<Unit>>;
