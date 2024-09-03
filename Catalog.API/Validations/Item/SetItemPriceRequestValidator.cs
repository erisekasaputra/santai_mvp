using Catalog.API.Applications.Commands.Items.SetItemPrice;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class SetItemPriceRequestValidator : AbstractValidator<SetItemPriceRequest>
{
    public SetItemPriceRequestValidator()
    {
        RuleFor(x => x.ItemId)
          .NotEmpty().WithMessage("Id is required.")
          .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount can not be null")
            .InclusiveBetween(1, decimal.MaxValue).WithMessage($"Amount must be in between 1 and {decimal.MaxValue}");
    }
}
