using Catalog.API.Applications.Commands.Categories.DeleteCategory;
using FluentValidation;

namespace Catalog.API.Validations.Category;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    { 
    }
}
