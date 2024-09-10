using Catalog.API.Applications.Commands.Items.SetItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class SetItemStockQuantityCommandValidator : AbstractValidator<SetItemStockQuantityCommand>
{
    public SetItemStockQuantityCommandValidator()
    { 
        RuleForEach(x => x.Items).SetValidator(new SetItemStockQuantityRequestValidator());
    }
}
