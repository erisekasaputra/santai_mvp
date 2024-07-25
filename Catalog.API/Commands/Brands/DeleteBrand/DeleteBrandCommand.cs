using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Brands.DeleteBrand;

public record DeleteBrandCommand(string Id) : IRequest<Result<Unit>>
{
}
