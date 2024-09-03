using Catalog.API.Applications.Commands.Items.CreateItem; 
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Sku) 
            .MaximumLength(50).WithMessage("Sku cannot exceed 50 characters.");

        RuleFor(x => x.Price)
            .InclusiveBetween(1, decimal.MaxValue).WithMessage($"Item price must be in between 0 and {decimal.MaxValue}");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters.")
            .NotEmpty().WithMessage("Image URL is required.")
            .Must(BeAValidUrl).WithMessage("Image URL must be a valid URL.");

        RuleFor(x => x.StockQuantity)
            .InclusiveBetween(0, int.MaxValue).WithMessage($"Stock quantity must be in between 0 and {int.MaxValue}");

        RuleFor(x => x.SoldQuantity)
            .InclusiveBetween(0, int.MaxValue).WithMessage($"Sold quantity must be in between 0 and {int.MaxValue}");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category Id is required.")
            .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");

        RuleFor(x => x.BrandId)
            .NotEmpty().WithMessage("Brand Id is required.") 
            .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");

        When(x => x.OwnerReviews is not null && x.OwnerReviews.Any(), () =>
        { 
            RuleForEach(e => e.OwnerReviews).SetValidator(new ItemOwnerReviewsCommandValidator()); 
        }); 
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    } 
}


