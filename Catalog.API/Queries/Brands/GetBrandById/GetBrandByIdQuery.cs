using Catalog.API.DTOs.BrandDto;
using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Queries.Brands.GetBrandById;

public record GetBrandByIdQuery(string Id) : IRequest<Result<BrandDto>>
{
}
