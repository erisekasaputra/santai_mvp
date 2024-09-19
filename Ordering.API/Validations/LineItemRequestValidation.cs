using FluentValidation;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Validations;

public class LineItemRequestValidation : AbstractValidator<LineItemRequest>
{
    public LineItemRequestValidation()
    {
        RuleFor(x => x.Id)
          .NotEmpty().WithMessage("Line item ID cannot be empty.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
