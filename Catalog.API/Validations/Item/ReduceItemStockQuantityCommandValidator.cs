using Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class ReduceItemStockQuantityCommandValidator : AbstractValidator<ReduceItemStockQuantityCommand>
{
    public ReduceItemStockQuantityCommandValidator()
    {
        RuleForEach(x => x.ItemIds).SetValidator(new ReduceItemStockQuantityRequestValidator());
    }
}
