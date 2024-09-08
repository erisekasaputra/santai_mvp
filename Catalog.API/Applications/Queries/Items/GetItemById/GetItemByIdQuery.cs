
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemById;

public record GetItemByIdQuery(Guid Id) : IRequest<Result>;