using Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class ReduceItemStockQuantityRequestValidator : AbstractValidator<ReduceItemStockQuantityRequest>
{
    public ReduceItemStockQuantityRequestValidator()
    {  
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity can not be null")
            .InclusiveBetween(1, int.MaxValue).WithMessage($"Quantity must be in between 1 and {int.MaxValue}");
    }
}
