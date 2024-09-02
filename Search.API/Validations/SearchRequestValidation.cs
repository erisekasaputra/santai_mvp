using FluentValidation;
using Search.API.Applications.Dto;

namespace Search.API.Validations;

public class SearchRequestValidation : AbstractValidator<SearchRequestDto>
{
    public SearchRequestValidation()
    {
        // CategoryId is required, must not be null, and must have a length of 26 characters
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.")
            .NotNull().WithMessage("CategoryId must not be null.")
            .Length(26).WithMessage("CategoryId must be exactly 26 characters long.");

        // BrandId is optional, but if provided, it must have a length of 26 characters
        RuleFor(x => x.BrandId)
            .Length(26).WithMessage("BrandId must be exactly 26 characters long.")
            .When(x => !string.IsNullOrEmpty(x.BrandId));

        // PageNumber must be greater than or equal to 1
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be 1 or greater.");
    }
}
