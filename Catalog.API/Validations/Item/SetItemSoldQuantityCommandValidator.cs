using Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class SetItemSoldQuantityCommandValidator : AbstractValidator<SetItemSoldQuantityCommand>
{
    public SetItemSoldQuantityCommandValidator()
    {
        RuleForEach(x => x.Items).SetValidator(new SetItemSoldQuantityRequestValidator());
    }
}
