using Catalog.API.Queries.Items.GetItemById;
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .Length(26).WithMessage("The item id should be in 26 character length");
    }
}
