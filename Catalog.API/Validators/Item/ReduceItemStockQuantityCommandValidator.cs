using Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class ReduceItemStockQuantityCommandValidator : AbstractValidator<ReduceItemStockQuantityCommand>
{
    public ReduceItemStockQuantityCommandValidator()
    {
        RuleForEach(x => x.ReduceItemStockQuantityRequests).SetValidator(new ReduceItemStockQuantityRequestValidator());
    }
}
