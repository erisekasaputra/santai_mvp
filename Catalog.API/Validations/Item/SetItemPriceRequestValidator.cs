using Catalog.API.Applications.Commands.Items.SetItemPrice;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class SetItemPriceRequestValidator : AbstractValidator<SetItemPriceRequest>
{
    public SetItemPriceRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is not allowed to be null")
            .Length(26).WithMessage("Item ID should in 26 characters length");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount can not be null")
            .InclusiveBetween(1, decimal.MaxValue).WithMessage($"Amount must be in between 1 and {decimal.MaxValue}");
    }
}
