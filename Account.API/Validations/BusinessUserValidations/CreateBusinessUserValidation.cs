using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using Account.API.Validations.BusinessLicenseValidations;
using Account.API.Validations.StaffValidations;
using Core.Extensions;
using Core.Validations; 
using FluentValidation;

namespace Account.API.Validations.BusinessUserValidations;

public class CreateBusinessUserValidation : AbstractValidator<BusinessUserRequestDto>
{
    public CreateBusinessUserValidation()
    {
        RuleFor(x => x.Password)
            .Must(PasswordValidation.IsValidPassword).WithMessage("Password must be at least 6 characters long and contain at least one digit, one lowercase letter, and one uppercase letter.");
         
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number can not be empty")
            .Length(3, 20).WithMessage("Phone number must be between 3 and 20 characters long")
            .Must(PhoneNumberValidation.IsValidPhoneNumber).WithMessage("Phone number must consist of number only and start with '+'");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
           .NotEmpty().WithMessage("Time zone can not be empty")
           .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
           .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone {PropertyValue} is invalid, please provide valid time zone");

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name can not be empty")
            .Length(3, 50).WithMessage("Business name must be between 3 and 50 characters long");

        RuleFor(x => x.BusinessImageUrl)
            .NotEmpty().WithMessage("Profile picture must not be empty");

        RuleFor(x => x.ContactPerson)
            .NotEmpty().WithMessage("Contact person can not be empty")
            .Length(3, 254).WithMessage("Contact person must be between 3 and 254 characters long");

        RuleFor(x => x.TaxId) 
            .Length(3, 50).WithMessage("Tax id must be between 3 and 50 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.TaxId));

        RuleFor(x => x.WebsiteUrl) 
            .Length(3, 255).WithMessage("Website url must be between 3 and 255 characters long")
            .Must(UrlValidation.IsValidUrl).WithMessage("Website url format is invalid")
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));

        RuleFor(x => x.BusinessDescription) 
            .Length(1, 1000).WithMessage("Business description must be between 1 and 1000 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.BusinessDescription));

        RuleFor(x => x.ReferralCode) 
            .Length(6).WithMessage("Referral code must be in 6 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.ReferralCode));

        When(x => x.BusinessLicenses is not null && x.BusinessLicenses.Any(), () =>
        {
            RuleForEach(e => e.BusinessLicenses).SetValidator(new BusinessLicenseValidation());
        });

        When(x => x.Staffs is not null && x.Staffs.Any(), () =>
        {
            RuleForEach(e => e.Staffs).SetValidator(new CreateStaffValidation());
        });
    } 
}
