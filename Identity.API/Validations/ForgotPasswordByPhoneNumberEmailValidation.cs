using FluentValidation;
using Identity.API.Dto;
using Identity.API.Extensions;

namespace Identity.API.Validations;

public class ForgotPasswordByPhoneNumberEmailValidation : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordByPhoneNumberEmailValidation()
    {
        RuleFor(x => x.Identity)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(PhoneNumberValidator.IsValid).WithMessage("Phone number must consist of digits only and start with a '+'.")
            .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");
    }
}
