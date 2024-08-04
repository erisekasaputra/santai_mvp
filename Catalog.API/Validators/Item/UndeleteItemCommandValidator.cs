using Catalog.API.Applications.Commands.Items.UndeleteItem;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class UndeleteItemCommandValidator : AbstractValidator<UndeleteItemCommand>
{
    public UndeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item ID is required.")
            .Length(26).WithMessage("The length of the item ID should be 26 characters.");
    }
}
