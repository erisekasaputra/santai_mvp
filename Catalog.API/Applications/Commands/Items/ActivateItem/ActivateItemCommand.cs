
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ActivateItem;

public record ActivateItemCommand(Guid Id) : IRequest<Result>;
