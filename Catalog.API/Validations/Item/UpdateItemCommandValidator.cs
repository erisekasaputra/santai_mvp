using Catalog.API.Applications.Commands.Items.UpdateItem;
using FluentValidation;

namespace Catalog.API.Validations.Item;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
          .NotEmpty().WithMessage("Id is required.")
          .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");

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
            .NotEmpty().WithMessage("Image URL is required.");

        RuleFor(x => x.StockQuantity)
            .InclusiveBetween(0, int.MaxValue).WithMessage($"Stock quantity must be in between 0 and {int.MaxValue}");

        RuleFor(x => x.SoldQuantity)
            .InclusiveBetween(0, int.MaxValue).WithMessage($"Sold quantity must be in between 0 and {int.MaxValue}");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category Id is required.")
            .NotEqual(Guid.Empty).WithMessage("The Id cannot be empty.");

        RuleFor(x => x.BrandId)
            .NotEmpty().WithMessage("Brand ID is required.")
            .NotEqual(Guid.Empty).WithMessage("The ID cannot be empty.");

        When(x => x.OwnerReviews is not null && x.OwnerReviews.Any(), () =>
        {
            RuleForEach(e => e.OwnerReviews).SetValidator(new ItemOwnerReviewsCommandValidator());
        });
    }
     
}
