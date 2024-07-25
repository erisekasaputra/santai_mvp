using Catalog.API.Queries.Categories.GetCategoryById;
using FluentValidation;

namespace Catalog.API.Validators.Category;

public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .Length(26).WithMessage("The category id should be in 26 character length");
    }
}
