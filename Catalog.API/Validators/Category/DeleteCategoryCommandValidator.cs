using Catalog.API.Commands.Categories.DeleteCategory;
using FluentValidation;

namespace Catalog.API.Validators.Category;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.")
            .Length(26).WithMessage("The length of the category ID should be 26 characters.");
    }
}
