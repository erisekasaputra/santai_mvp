using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.UndeleteItem;

public record UndeleteItemCommand(Guid Id) : IRequest<Result>;