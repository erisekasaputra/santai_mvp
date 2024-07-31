using Catalog.API.DTOs.OwnerReview;  
using FluentValidation;

namespace Catalog.API.Validators.Item;

public class ItemOwnerReviewsCommandValidator : AbstractValidator<OwnerReviewDto>
{
    public ItemOwnerReviewsCommandValidator()
    {
        RuleFor(x => x.Title)
               .NotEmpty().WithMessage("Title is required")
               .Length(1, 25).WithMessage("Title must be between 1 and 25 characters.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 10).WithMessage("Rating must be between 0 and 10"); ;
    }
}
