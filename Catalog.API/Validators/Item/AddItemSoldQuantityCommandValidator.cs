using Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class AddItemSoldQuantityCommandValidator : AbstractValidator<AddItemSoldQuantityCommand>
{
    public AddItemSoldQuantityCommandValidator()
    {
        RuleForEach(x => x.AddItemSoldQuantityRequests).SetValidator(new AddItemSoldQuantityRequestValidator());
    }
}
