using Account.API.Applications.Dtos.RequestDtos; 
using FluentValidation; 

namespace Account.API.Validations.FleetValidations;

public class UpdateFleetValidation : AbstractValidator<UpdateFleetRequestDto>
{
    public UpdateFleetValidation()
    { 
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Make is required.")
            .MaximumLength(100).WithMessage("Make can not exceed more than 100 characters");

        // Model: Required
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model can not exceed more than 100 characters");

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage("Image resource name can not be empty");
    }
}
