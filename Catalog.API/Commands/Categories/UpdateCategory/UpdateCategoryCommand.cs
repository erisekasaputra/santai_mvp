using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Categories.UpdateCategory;

public record UpdateCategoryCommand(
    string Id,
    string Name,
    string Description,
    string ImageUrl) : IRequest<Result<Unit>>
{
}
