using Catalog.API.Applications.Commands.Items.ActivateItem;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class ActivateItemCommandValidator : AbstractValidator<ActivateItemCommand>
{
    public ActivateItemCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.")
           .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty."); 
    }
}
