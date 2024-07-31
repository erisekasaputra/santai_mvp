using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.DeleteBrand;

public record DeleteBrandCommand(string Id) : IRequest<Result<Unit>>;