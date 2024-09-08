
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result>;