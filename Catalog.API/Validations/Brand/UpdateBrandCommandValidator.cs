using Catalog.API.Applications.Commands.Brands.UpdateBrand;
using FluentValidation;

namespace Catalog.API.Validations.Brand;

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandCommandValidator()
    {   
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters.")
            .NotEmpty().WithMessage("Image URL is required.");
    } 
}
