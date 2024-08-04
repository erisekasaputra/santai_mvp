using Catalog.API.DTOs.Category;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string ImageUrl,
    bool IsActive) : IRequest<Result<CategoryDto>>;