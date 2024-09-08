using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string ImageUrl,
    bool IsActive) : IRequest<Result>;