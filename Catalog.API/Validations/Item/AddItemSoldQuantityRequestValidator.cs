using Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class AddItemSoldQuantityRequestValidator : AbstractValidator<AddItemSoldQuantityRequest>
{
    public AddItemSoldQuantityRequestValidator()
    {  
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity can not be null")
            .InclusiveBetween(1, int.MaxValue).WithMessage($"Quantity must be in between 1 and {int.MaxValue}");
    }
}
