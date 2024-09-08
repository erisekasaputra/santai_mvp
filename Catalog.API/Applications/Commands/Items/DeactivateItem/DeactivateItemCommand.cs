using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeactivateItem;

public record DeactivateItemCommand(Guid Id) : IRequest<Result>;
