using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.UpdateBrand;

public record UpdateBrandCommand(
    Guid Id,
    string Name,
    string ImageUrl,
    bool IsActive) : IRequest<Result>;
