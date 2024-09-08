using Catalog.API.Applications.Commands.Items.SetItemPrice;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class SetItemPriceCommandValidator : AbstractValidator<SetItemPriceCommand>
{
    public SetItemPriceCommandValidator()
    { 
        RuleForEach(x => x.ItemIds).SetValidator(new SetItemPriceRequestValidator());
    }
}
