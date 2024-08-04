using Catalog.API.Applications.Commands.Items.SetItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class SetItemStockQuantityCommandValidator : AbstractValidator<SetItemStockQuantityCommand>
{
    public SetItemStockQuantityCommandValidator()
    {
        RuleForEach(x => x.SetItemStockQuantityRequests).SetValidator(new SetItemStockQuantityRequestValidator());
    }
}
