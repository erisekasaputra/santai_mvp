using Core.Validations;
using FluentValidation;
using Identity.API.Applications.Dto; 

namespace Identity.API.Validations;

public class LoginStaffValidation : AbstractValidator<LoginStaffRequest>
{
    public LoginStaffValidation()
    {  
        RuleFor(p => p.BusinessCode)
            .NotEmpty().WithMessage("Business code must not be empty")
            .Length(6).WithMessage("Business code should in 6 characters length");

        RuleFor(x => x.RegionCode)
           .NotEmpty().WithMessage("Region code is required.")
           .Length(2).WithMessage("Region code must be exactly 2 characters long.")
           .Matches(@"^[A-Z]{2}$").WithMessage("Region code must consist of two uppercase letters.");

        RuleFor(p => p.PhoneNumber)
             .NotEmpty().WithMessage("Phone number is required.")
             .Must(PhoneNumberValidation.IsValidPhoneNumber).WithMessage("Phone number must consist of digits only")
             .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
             .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");
    }
}
