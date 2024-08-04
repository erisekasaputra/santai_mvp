using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.UpdateCategory;

public record UpdateCategoryCommand(
    string Id,
    string Name, 
    string ImageUrl,
    bool IsActive) : IRequest<Result<Unit>>;