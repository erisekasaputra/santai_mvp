using Catalog.API.DTOs.Category;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;