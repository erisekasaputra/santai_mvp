using FluentValidation;
using Ordering.API.Applications.Dtos.Requests;

namespace Ordering.API.Validations;

public class FleetRequestValidation : AbstractValidator<FleetRequest>
{
    public FleetRequestValidation()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Fleet ID cannot be empty.");
    }
}
