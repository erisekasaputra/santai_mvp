using Catalog.API.Applications.Queries.Brands.GetBrandById;
using FluentValidation;

namespace Catalog.API.Validations.Brand;

public class GetBrandByIdQueryValidator : AbstractValidator<GetBrandByIdQuery>
{
    public GetBrandByIdQueryValidator()
    { 
    }
}
