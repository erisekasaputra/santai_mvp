using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string ImageUrl) : IRequest<Result<CreateCategoryResponse>>
{
}
