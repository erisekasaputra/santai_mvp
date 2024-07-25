using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Brands.UpdateBrand;

public record UpdateBrandCommand(
    string Id,
    string Name,
    string ImageUrl) : IRequest<Result<Unit>>
{
}
