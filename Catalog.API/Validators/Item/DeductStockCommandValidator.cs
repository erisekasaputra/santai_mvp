using Catalog.API.Applications.Commands.Items.ReduceStock;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class DeductStockCommandValidator : AbstractValidator<ReduceStockCommand>
{
    public DeductStockCommandValidator()
    {
        RuleForEach(e => e.ItemStocks).SetValidator(new ItemStockCommandValidator());
    }
}
