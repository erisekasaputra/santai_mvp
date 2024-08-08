using FluentValidation;
using Catalog.API.Applications.Queries.Brands.GetBrandPaginated;

namespace Catalog.API.Validators.Brand;

public class GetBrandPaginatedQueryValidator : AbstractValidator<GetBrandPaginatedQuery>
{
    public GetBrandPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .InclusiveBetween(1, int.MaxValue).WithMessage($"Page number must be in between 1 and {int.MaxValue}");

        RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage($"Page number must be in between 1 and 100");
    }
}
