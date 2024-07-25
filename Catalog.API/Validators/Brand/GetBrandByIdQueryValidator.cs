using Catalog.API.Queries.Brands.GetBrandById;
using FluentValidation;

namespace Catalog.API.Validators.Brand;

public class GetBrandByIdQueryValidator : AbstractValidator<GetBrandByIdQuery>
{
    public GetBrandByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .Length(26).WithMessage("The brand id should be in 26 character length");
    }
}
