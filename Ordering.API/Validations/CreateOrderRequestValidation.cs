using FluentValidation;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Validations;

public class CreateOrderRequestValidation : AbstractValidator<OrderRequest>
{
    public CreateOrderRequestValidation()
    { 
        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line cannot be empty.")
            .Length(5, 100).WithMessage("Address line must be between 5 and 100 characters.");

        // Latitude and Longitude validation
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees.");

        // Currency validation
        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Invalid currency.");

        // IsScheduled and ScheduledAt validation
        RuleFor(x => x.ScheduledAt)
            .NotNull().When(x => x.IsScheduled)
            .WithMessage("Scheduled date must be set if scheduled.");

        // LineItems validation
        RuleForEach(x => x.LineItems).SetValidator(new LineItemRequestValidation());

        // Fleets validation
        RuleForEach(x => x.Fleets).SetValidator(new FleetRequestValidation());
    }
}
