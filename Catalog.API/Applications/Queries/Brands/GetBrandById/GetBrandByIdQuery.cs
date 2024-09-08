
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Brands.GetBrandById;

public record GetBrandByIdQuery(Guid Id) : IRequest<Result>;