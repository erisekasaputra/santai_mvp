using Catalog.API.Applications.Commands.Categories.UpdateCategory;
using FluentValidation;

namespace Catalog.API.Validations.Category;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Category ID is required.")
           .Length(26).WithMessage("The length of the category ID should be 26 characters.");

        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");
          
        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters.")
            .NotEmpty().WithMessage("Image URL is required.")
            .Must(BeAValidUrl).WithMessage("Image URL must be a valid URL."); 
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
