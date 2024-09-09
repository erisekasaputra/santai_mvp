using Core.Validations;
using FluentValidation;
using Identity.API.Applications.Dto;
using Identity.API.Extensions;

namespace Identity.API.Validations;

public class LoginUserValidation : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidation()
    { 
        RuleFor(p => p.RegionCode)
            .NotEmpty().WithMessage("Region code must not be empty")
            .Length(2).WithMessage("Region code should in 2 characters length");

        RuleFor(p => p.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(PhoneNumberValidation.IsValidPhoneNumber).WithMessage("Phone number must consist of digits only and start with a '+'.")
            .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");
    }
}
