using Catalog.API.Applications.Queries.Categories.GetCategoryById;
using FluentValidation;

namespace Catalog.API.Validations.Category;

public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    { 
    }
}
