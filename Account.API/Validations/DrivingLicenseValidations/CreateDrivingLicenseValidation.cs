using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Core.Validations;
using FluentValidation;

namespace Account.API.Validations.DrivingLicenseValidations;

public class CreateDrivingLicenseValidation : AbstractValidator<DrivingLicenseRequestDto>
{
    public CreateDrivingLicenseValidation()
    {
        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number can not be null")
            .Length(3, 40).WithMessage("The license number must be between 3 and 40 characters long");

        RuleFor(x => x.FrontSideImageUrl)
            .NotEmpty().WithMessage("Front side image url can not be empty");

        RuleFor(x => x.BackSideImageUrl)
            .NotEmpty().WithMessage("Back side image url can not be empty");
    }
}
