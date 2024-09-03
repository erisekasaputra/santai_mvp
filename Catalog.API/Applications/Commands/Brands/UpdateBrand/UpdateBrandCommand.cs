using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.UpdateBrand;

public record UpdateBrandCommand(
    Guid Id,
    string Name,
    string ImageUrl,
    bool IsActive) : IRequest<Result<Unit>>;
