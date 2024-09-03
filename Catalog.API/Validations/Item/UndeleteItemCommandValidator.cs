using Catalog.API.Applications.Commands.Items.UndeleteItem;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class UndeleteItemCommandValidator : AbstractValidator<UndeleteItemCommand>
{
    public UndeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.")
           .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");
    }
}
