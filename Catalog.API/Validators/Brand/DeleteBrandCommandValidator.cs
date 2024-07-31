using Catalog.API.Applications.Commands.Brands.DeleteBrand;
using FluentValidation;

namespace Catalog.API.Validators.Brand;

public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
{
    public DeleteBrandCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Brand ID is required.")
            .Length(26).WithMessage("The length of the brand ID should be 26 characters.");
    }
}
