using Catalog.API.Applications.Commands.Items.DeactivateItem;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class DeactivateItemCommandValidator : AbstractValidator<DeactivateItemCommand>
{
    public DeactivateItemCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.")
           .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");
    }
}
