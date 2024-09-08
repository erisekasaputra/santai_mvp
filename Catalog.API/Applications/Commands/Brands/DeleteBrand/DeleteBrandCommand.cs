using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.DeleteBrand;

public record DeleteBrandCommand(Guid Id) : IRequest<Result>;