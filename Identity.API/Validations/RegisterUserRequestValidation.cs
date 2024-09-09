
using Core.Validations;
using FluentValidation;
using Identity.API.Applications.Dto;

namespace Identity.API.Validations;

public class RegisterUserRequestValidation : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidation()
    { 
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(PhoneNumberValidation.IsValidPhoneNumber).WithMessage("Phone number must consist of digits only start with '+'.")
            .MinimumLength(8).WithMessage("Phone number must be at least 8 digits long.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 digits.");

        RuleFor(x => x.Password)
            .Must(PasswordValidation.IsValidPassword).WithMessage("Password must be at least 6 characters long and contain at least one digit, one lowercase letter, and one uppercase letter.");
         
        RuleFor(x => x.RegionCode)
            .NotEmpty().WithMessage("Region code is required.")
            .Length(2).WithMessage("Region code must be exactly 2 characters long.")
            .Matches(@"^[A-Z]{2}$").WithMessage("Region code must consist of two uppercase letters.");
         
        RuleFor(x => x.UserType)
            .IsInEnum().WithMessage("User type is not valid.");  
         
        RuleFor(x => x.ReturnUrl)
            .Must(BeAValidUrl).WithMessage("Return URL must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.ReturnUrl));
    }

    private bool BeAValidUrl(string? url)
    {
        if (url is null)
        {
            return true;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
