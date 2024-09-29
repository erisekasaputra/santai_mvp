using Catalog.API.Applications.Commands.Categories.CreateCategory;
using FluentValidation;

namespace Catalog.API.Validations.Category;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters.")
            .NotEmpty().WithMessage("Image URL is required.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
