using Catalog.API.DTOs.Category;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryById;

public record GetCategoryByIdQuery(string Id) : IRequest<Result<CategoryDto>>;