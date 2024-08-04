using Catalog.API.Applications.Commands.Items.ReduceItemSoldQuantity;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class ReduceItemSoldQuantityCommandValidator : AbstractValidator<ReduceItemSoldQuantityCommand>
{
    public ReduceItemSoldQuantityCommandValidator()
    {
        RuleForEach(x => x.ReduceItemSoldQuantityRequests).SetValidator(new ReduceItemSoldQuantityRequestValidator());
    }
}
