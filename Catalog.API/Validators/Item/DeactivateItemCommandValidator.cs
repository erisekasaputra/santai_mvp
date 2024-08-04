using Catalog.API.Applications.Commands.Items.DeactivateItem;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class DeactivateItemCommandValidator : AbstractValidator<DeactivateItemCommand>
{
    public DeactivateItemCommandValidator()
    { 
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required")
            .Length(26).WithMessage("The ID should be in 26 characters length");
    }
}
