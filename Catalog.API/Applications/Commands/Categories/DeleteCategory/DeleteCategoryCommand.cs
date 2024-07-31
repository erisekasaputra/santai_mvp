using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.DeleteCategory;

public record DeleteCategoryCommand(string Id) : IRequest<Result<Unit>>;