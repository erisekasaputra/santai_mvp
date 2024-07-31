using Catalog.API.DTOs.ItemStock;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class ItemStockCommandValidator : AbstractValidator<ItemStockDto>
{
    public ItemStockCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required.")
            .Length(26).WithMessage("The length of the item ID should be 26 characters.");

        RuleFor(x => x.QuantityDeduct)
           .InclusiveBetween(0, int.MaxValue).WithMessage($"Deduct quantity must be in between 0 and {int.MaxValue}");
    }
}
