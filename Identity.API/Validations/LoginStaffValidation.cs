using FluentValidation;
using Identity.API.Dto;
using Identity.API.Extensions;

namespace Identity.API.Validations;

public class LoginStaffValidation : AbstractValidator<LoginStaffRequest>
{
    public LoginStaffValidation()
    {  
        RuleFor(p => p.BusinessCode)
            .NotEmpty().WithMessage("Business code must not be empty")
            .Length(6).WithMessage("Business code should in 6 characters length");
        
        RuleFor(p => p.RegionCode)
            .NotEmpty().WithMessage("Region code must not be empty")
            .Length(2).WithMessage("Region code should in 2 characters length");

        RuleFor(p => p.PhoneNumber)
             .NotEmpty().WithMessage("Phone number is required.")
             .Must(PhoneNumberValidator.IsValid).WithMessage("Phone number must consist of digits only and may start with a '+'.")
             .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
             .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");
    }
}
