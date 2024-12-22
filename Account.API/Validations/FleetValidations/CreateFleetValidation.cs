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

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Plate number cannot be empty.")
            .Matches(@"(^[A-Z]{1,3}\s*[0-9]{1,4}\s*[A-Z]?$)|(^Q[A-Z]{1,2}\s*[0-9]{1,4}\s*[A-Z]?$)|(^KV\s*[0-9]{1,4}\s*[A-Z]?$)|(^H[A-Z]{1,2}\s*[0-9]{1,4}$)|(^[0-9]{1,4}\s*CD|[0-9]{1,2}\s*CD\s*[0-9]{1,2}$)|(^Z[A-Z]?\s*[0-9]{1,4}$)|(^DYMM|Raja|SUK\s*[0-9]{1,4}$)|(^[A-Z]\s*[0-9]{1,4}\s*[A-Z]$)")
            .WithMessage("Plate number should match one of the following formats: 'W 1234 A', 'W1234A', 'ABC 1234', 'QA 1234 A', 'QAB 1234', 'KV 1234 A', 'HBA 1234', '12 CD 34', 'Z 1234', 'DYMM 1234', 'P 1234 A'.");
    } 
}
