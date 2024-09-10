using Catalog.API.Applications.Commands.Items.AddItemStockQuantity;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class AddItemStockQuantityCommandValidator : AbstractValidator<AddItemStockQuantityCommand>
{
    public AddItemStockQuantityCommandValidator()
    {
        RuleForEach(x => x.Items).SetValidator(new AddItemStockQuantityRequestValidator());
    }
}
