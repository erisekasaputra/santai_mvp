using Catalog.API.Applications.Commands.Items.ActivateItem;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class ActivateItemCommandValidator : AbstractValidator<ActivateItemCommand>
{
    public ActivateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required")
            .Length(26).WithMessage("The ID should be in 26 characters long");
    }
}
