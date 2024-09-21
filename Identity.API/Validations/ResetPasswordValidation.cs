using Core.Validations;
using FluentValidation;
using Identity.API.Applications.Dto;

namespace Identity.API.Validations;

public class ResetPasswordValidation : AbstractValidator<PasswordResetRequest>
{
    public ResetPasswordValidation()
    {
        RuleFor(x => x.Identity)
            .NotEmpty().WithMessage("Identity can not be empty");

        RuleFor(x => x.NewPassword)
           .Must(PasswordValidation.IsValidPassword).WithMessage("Password must be at least 6 characters long and contain at least one digit, one lowercase letter, and one uppercase letter.");

        RuleFor(x => x.OtpCode)
            .Length(6).WithMessage("Otp must be in 6 characters length");
    }
}
