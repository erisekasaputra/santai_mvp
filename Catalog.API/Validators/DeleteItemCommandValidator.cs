using Catalog.API.Commands.DeleteItem;
using FluentValidation;

namespace Catalog.API.Validators;

public class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item ID is required.")
            .Length(26).WithMessage("The length of the item ID should be 26 characters.");
    }
}
