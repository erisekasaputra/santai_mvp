using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Brands.CreateBrand;

public record CreateBrandCommand(
    string Name,
    string ImageUrl) : IRequest<Result<CreateBrandResponse>>
{
}
