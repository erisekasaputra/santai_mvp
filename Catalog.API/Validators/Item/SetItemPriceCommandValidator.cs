using Catalog.API.Applications.Commands.Items.SetItemPrice;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class SetItemPriceCommandValidator : AbstractValidator<SetItemPriceCommand>
{
    public SetItemPriceCommandValidator()
    {
        RuleForEach(x => x.SetItemPriceRequests).SetValidator(new SetItemPriceRequestValidator());
    }
}
