using Catalog.API.Applications.Commands.Items.AddItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class AddItemStockQuantityRequestValidator : AbstractValidator<AddItemStockQuantityRequest>
{
    public AddItemStockQuantityRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is not allowed to be null")
            .Length(26).WithMessage("Item ID should in 26 characters length");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity can not be null")
            .InclusiveBetween(1, int.MaxValue).WithMessage($"Quantity must be in between 1 and {int.MaxValue}");
    }
}
