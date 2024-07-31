using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.UpdateBrand;

public record UpdateBrandCommand(
    string Id,
    string Name,
    string ImageUrl) : IRequest<Result<Unit>>;
