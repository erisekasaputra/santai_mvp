using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Commands.Categories.DeleteCategory;

public record DeleteCategoryCommand(string Id) : IRequest<Result<Unit>>
{
}
