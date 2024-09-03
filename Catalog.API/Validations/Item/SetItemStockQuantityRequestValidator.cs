using Catalog.API.Applications.Commands.Items.SetItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class SetItemStockQuantityRequestValidator : AbstractValidator<SetItemStockQuantityRequest>
{
    public SetItemStockQuantityRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Id is required.")
            .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity can not be null")
            .InclusiveBetween(1, int.MaxValue).WithMessage($"Quantity must be in between 1 and {int.MaxValue}");
    }
}
