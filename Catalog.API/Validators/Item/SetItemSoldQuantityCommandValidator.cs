using Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class SetItemSoldQuantityCommandValidator : AbstractValidator<SetItemSoldQuantityCommand>
{
    public SetItemSoldQuantityCommandValidator()
    {
        RuleForEach(x => x.SetItemSoldQuantityRequests).SetValidator(new SetItemSoldQuantityRequestValidator());
    }
}
