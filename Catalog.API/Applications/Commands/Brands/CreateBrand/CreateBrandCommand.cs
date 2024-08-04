using Catalog.API.DTOs.Brand;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.CreateBrand;

public record CreateBrandCommand(
    string Name,
    string ImageUrl,
    bool IsActive) : IRequest<Result<BrandDto>>;
