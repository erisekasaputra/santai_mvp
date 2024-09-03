using Catalog.API.Applications.Commands.Brands.DeleteBrand;
using FluentValidation;

namespace Catalog.API.Validations.Brand;

public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
{
    public DeleteBrandCommandValidator()
    { 
    }
}
