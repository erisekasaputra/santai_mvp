using FluentValidation;
using Catalog.API.Applications.Queries.Items.GetItemPaginated;

namespace Catalog.API.Validators.Item;

public class GetItemPaginatedQueryValidator : AbstractValidator<GetItemPaginatedQuery>
{
    public GetItemPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .InclusiveBetween(1, int.MaxValue).WithMessage($"Page number must be in between 1 and {int.MaxValue}");

        RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage($"Page number must be in between 1 and 100");
    }
}
