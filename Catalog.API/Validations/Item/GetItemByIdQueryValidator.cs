using Catalog.API.Applications.Queries.Items.GetItemById;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.")
           .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");
    }
}
