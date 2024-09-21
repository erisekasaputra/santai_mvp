using FluentValidation;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Validations;

public class PaymentValidation : AbstractValidator<SenangPayPaymentRequest>
{
    public PaymentValidation()
    {
        RuleFor(x => x.OrderId)
           .NotEqual(Guid.Empty).WithMessage("OrderId must be a valid non-empty GUID."); 

        // Validate Amount (must be greater than 0)
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        // Validate Method (required and must not be empty)
        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required.")
            .MaximumLength(50).WithMessage("Reference must be at least 5 characters long."); 

        // Validate Hash (required)
        RuleFor(x => x.Hash)
            .NotEmpty().WithMessage("Hash is required.");

        // Validate Status (must be either 0 or 1)
        RuleFor(x => x.Status)
            .InclusiveBetween(0, 10).WithMessage("Status must be either 0 (failed) or 1 (successful) or 3 (pending authorization).");
    }
}
