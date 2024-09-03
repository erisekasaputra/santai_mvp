using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using FluentValidation; 

namespace Account.API.Validations.FleetValidations;

public class UpdateFleetValidation : AbstractValidator<UpdateFleetRequestDto>
{
    public UpdateFleetValidation()
    {
        // Registration Number: Alphanumeric and required
        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required.") 
            .MaximumLength(30).WithMessage("Registration number can not exceed more than 30 characters");

        // Vehicle Type: Should be one of the predefined types (e.g., Car, Motorcycle, Truck)
        RuleFor(x => x.VehicleType)
            .NotEmpty().WithMessage("Vehicle type is required.")
            .IsInEnum().WithMessage("Invalid vehicle type.");

        // Make: Required and should be a valid brand
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Make is required.")
            .MaximumLength(100).WithMessage("Make can not exceed more than 100 characters");

        // Model: Required
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model can not exceed more than 100 characters");

        // Year of Manufacture: Should be a valid year, reasonable range (e.g., last 100 years)
        RuleFor(x => x.YearOfManufacture)
            .InclusiveBetween(1990, DateTime.UtcNow.Year)
            .WithMessage($"Invalid year of manufacture, year must be between '1990' and '{DateTime.UtcNow.Year}'.");

        // Chassis Number: Required, alphanumeric
        RuleFor(x => x.ChassisNumber)
            .NotEmpty().WithMessage("Chassis number is required.") 
            .MaximumLength(100).WithMessage("Chassis number can not exceed more than 100 characters");

        // Engine Number: Required, alphanumeric
        RuleFor(x => x.EngineNumber)
            .NotEmpty().WithMessage("Engine number is required.") 
            .MaximumLength(100).WithMessage("Engine number can not exceed more than 100 characters");

        // Insurance Number: Required, alphanumeric
        RuleFor(x => x.InsuranceNumber)
            .NotEmpty().WithMessage("Insurance number is required.") 
            .MaximumLength(100).WithMessage("Insurance number can not exceed more than 100 characters");

        // Is Insurance Valid: Required (Boolean)
        RuleFor(x => x.IsInsuranceValid)
            .NotNull().WithMessage("Insurance validity status is required.");

        // Odometer Reading: Non-negative integer
        RuleFor(x => x.OdometerReading)
            .GreaterThanOrEqualTo(0).WithMessage("Odometer reading must be non-negative.");

        // Fuel Type: Should be one of the predefined types (e.g., Petrol, Diesel, Electric)
        RuleFor(x => x.FuelType)
            .NotEmpty().WithMessage("Fuel type is required.")
            .IsInEnum().WithMessage("Invalid fuel type. Should be Petrol, Diesel, Electric"); 

        // Owner Name: Required
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required.")
            .Must(NameExtension.IsValidName).WithMessage("Owner name must not contains any number or special characters")
            .MaximumLength(50).WithMessage("Owner name can not exceed more than 50 characters");

        // Owner Address: Required
        RuleFor(x => x.OwnerAddress)
            .NotEmpty().WithMessage("Owner address is required.")
            .MaximumLength(255).WithMessage("Owner address can not exceed more than 255 characters");

        // Usage Status: Should be one of the predefined types (e.g., Private, Commercial)
        RuleFor(x => x.UsageStatus)
            .NotEmpty().WithMessage("Usage status is required.")
            .IsInEnum().WithMessage("Invalid usage status. Should be Private, Commercial etc");

        // Ownership Status: Should be one of the predefined types (e.g., Owned, Leased)
        RuleFor(x => x.OwnershipStatus)
            .NotEmpty().WithMessage("Ownership status is required.")
            .IsInEnum().WithMessage("Invalid ownership status. Should be Owned, Leased, Rental etc");

        // Transmission Type: Should be one of the predefined types (e.g., Manual, Automatic)
        RuleFor(x => x.TransmissionType)
            .NotEmpty().WithMessage("Transmission type is required.")
            .IsInEnum().WithMessage("Invalid transmission type. Should be Manual or Automatic.");

        RuleFor(x => x.ImageUrl)
            .Must(url =>
                string.IsNullOrWhiteSpace(url) || UrlExtension.IsValidImageUrl(url))
            .WithMessage("Image url format is invaid");
    }
}
