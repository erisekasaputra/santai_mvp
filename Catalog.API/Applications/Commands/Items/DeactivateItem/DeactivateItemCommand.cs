using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeactivateItem;

public record DeactivateItemCommand(Guid Id) : IRequest<Result<Unit>>;
