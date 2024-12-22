using Account.API.Applications.Dtos.RequestDtos; 
using FluentValidation;
using System.Text.RegularExpressions;

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
            .Matches("^[A-Z]{1,3}\\s*[0-9]{1,4}\\s*[A-Z]?$")
            .WithMessage("Plate number should be in the format: 'W 1234 A', 'W1234A', or 'ABC 1234'.")
            .Matches("^Q[A-Z]{1,2}\\s*[0-9]{1,4}\\s*[A-Z]?$")
            .WithMessage("Sarawak plate should be in the format: 'QA 1234 A', or 'QAB 1234'.")
            .Matches("^KV\\s*[0-9]{1,4}\\s*[A-Z]?$")
            .WithMessage("Langkawi plate should be in the format: 'KV 1234 A'.")
            .Matches("^H[A-Z]{1,2}\\s*[0-9]{1,4}$")
            .WithMessage("Taxi plate should be in the format: 'HBA 1234' or 'HWD 1234'.")
            .Matches("^([0-9]{1,4}\\s*CD|[0-9]{1,2}\\s*CD\\s*[0-9]{1,2})$")
            .WithMessage("Diplomatic plate should be in the format: '1234 CD', '12 CD 34'.")
            .Matches("^Z[A-Z]?\\s*[0-9]{1,4}$")
            .WithMessage("Military plate should be in the format: 'Z 1234' or 'ZD 1234'.")
            .Matches("^(DYMM|Raja|SUK)\\s*[0-9]{1,4}$")
            .WithMessage("Royal plate should be in the format: 'DYMM 1', 'SUK 1234'.")
            .Matches("^[A-Z]\\s*[0-9]{1,4}\\s*[A-Z]$")
            .WithMessage("Trade plate should be in the format: 'P 1234 A', 'S 1234 A'.");
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
