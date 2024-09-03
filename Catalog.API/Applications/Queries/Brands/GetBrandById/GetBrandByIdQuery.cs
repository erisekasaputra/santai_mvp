using Catalog.API.DTOs.Brand;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Brands.GetBrandById;

public record GetBrandByIdQuery(Guid Id) : IRequest<Result<BrandDto>>
{
}
