using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name, 
    string ImageUrl,
    bool IsActive) : IRequest<Result>;