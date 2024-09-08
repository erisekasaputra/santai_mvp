using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;