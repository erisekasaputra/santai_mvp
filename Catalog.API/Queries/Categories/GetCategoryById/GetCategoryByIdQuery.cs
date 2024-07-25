using Catalog.API.DTOs.CategoryDto;
using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Queries.Categories.GetCategoryById;

public record GetCategoryByIdQuery(string Id) : IRequest<Result<CategoryDto>>;