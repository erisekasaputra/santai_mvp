using Catalog.API.Queries.GetItemById;
using FluentValidation;

namespace Catalog.API.Validators;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .Length(26).WithMessage("The item id should be in 26 character length");
    }
}
