using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.DeleteBrand;

public record DeleteBrandCommand(Guid Id) : IRequest<Result<Unit>>;