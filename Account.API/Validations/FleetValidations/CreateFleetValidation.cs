using Account.API.Applications.Dtos.RequestDtos; 
using FluentValidation;

namespace Account.API.Validations.FleetValidations;

public class CreateFleetValidation : AbstractValidator<CreateFleetRequestDto>
{
    public CreateFleetValidation()
    { 
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100).WithMessage("Brand can not exceed more than 100 characters");

        // Model: Required
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model can not exceed more than 100 characters"); 

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage("Image resource name can not be empty");
    }
}
