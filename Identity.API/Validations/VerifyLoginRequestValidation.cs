using Core.Validations;
using FluentValidation;
using Identity.API.Applications.Dto;

namespace Identity.API.Validations;

public class VerifyLoginRequestValidation : AbstractValidator<VerifyLoginRequest>
{
    public VerifyLoginRequestValidation()
    {
        RuleFor(p => p.Token)
            .NotEmpty().WithMessage("Token must not be empty")
            .Length(6).WithMessage("Invalid token format");

        RuleFor(p => p.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(PhoneNumberValidation.IsValidPhoneNumber).WithMessage("Phone number must consist of digits only.")
            .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");

        RuleFor(p => p.DeviceId)
            .NotEmpty().WithMessage("Device token must not be empty");
    }
}
