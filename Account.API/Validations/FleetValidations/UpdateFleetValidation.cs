using Account.API.Applications.Dtos.RequestDtos; 
using FluentValidation;
using System.Text.RegularExpressions;

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

        RuleFor(x => x.RegistrationNumber)
            .NotNull().WithMessage("Plate number cannot be null.")
            .NotEmpty().WithMessage("Plate number cannot be empty.")
            .Must(BeAValidMalaysianPlate).WithMessage("Invalid plate number.");
    }

    private bool BeAValidMalaysianPlate(string? plateNumber)
    {
        if (string.IsNullOrWhiteSpace(plateNumber)) return false;

        // Define regex patterns for various plate types
        var patterns = new[]
        {
            // Standard plate (e.g., W1234A, ABC1234, W123A)
            "^[A-Z]{1,3}[0-9]{1,4}[A-Z]?$",

            // Sarawak plate (e.g., QA1234A, QAB1234)
            "^Q[A-Z]{1,2}[0-9]{1,4}[A-Z]?$",

            // Langkawi plate (e.g., KV1234A)
            "^KV[0-9]{1,4}[A-Z]?$",

            // Taxi plate (e.g., HBA1234, HWD1234)
            "^H[A-Z]{1,2}[0-9]{1,4}$",

            // Diplomatic plate (e.g., 1234CD, 12CD34)
            "^([0-9]{1,4}CD|[0-9]{1,2}CD[0-9]{1,2})$",

            // Military plate (e.g., Z1234, ZD1234)
            "^Z[A-Z]?[0-9]{1,4}$",

            // Royal plate (e.g., DYMM1, SUK1234)
            "^(DYMM|Raja|SUK)[0-9]{1,4}$",

            // Trade plates (e.g., P1234A, S1234A)
            "^[A-Z][0-9]{1,4}[A-Z]$",
        };

        // Check if the plate number matches any pattern
        foreach (var pattern in patterns)
        {
            if (Regex.IsMatch(plateNumber, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
