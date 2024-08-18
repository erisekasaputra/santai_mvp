using FluentValidation;
using Identity.API.Dto;
using Identity.API.Extensions;

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
            .Must(PhoneNumberValidator.IsValid).WithMessage("Phone number must consist of digits only and may start with a '+'.")
            .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");
    }
}
