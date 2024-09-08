using Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class AddItemSoldQuantityCommandValidator : AbstractValidator<AddItemSoldQuantityCommand>
{
    public AddItemSoldQuantityCommandValidator()
    {
        RuleForEach(x => x.ItemIds).SetValidator(new AddItemSoldQuantityRequestValidator());
    }
}
