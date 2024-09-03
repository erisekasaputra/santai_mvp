using Catalog.API.Applications.Commands.Items.DeleteItem;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.")
           .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");
    }
}
