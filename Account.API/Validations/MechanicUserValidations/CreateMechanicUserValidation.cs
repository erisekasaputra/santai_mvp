using Account.API.Applications.Dtos.RequestDtos; 
using Account.API.Validations.AddressValidations; 
using Account.API.Validations.CertificationValidations;
using Account.API.Validations.DrivingLicenseValidations;
using Account.API.Validations.NationalIdentityValidations;
using Account.API.Validations.PersonalInfoValidations;
using Core.Extensions;
using FluentValidation;

namespace Account.API.Validations.MechanicUserValidations;

public class CreateMechanicUserValidation : AbstractValidator<MechanicUserRequestDto>
{
    public CreateMechanicUserValidation()
    {  
        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
           .NotEmpty().WithMessage("Time zone can not be empty")
           .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
           .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone {PropertyValue} is invalid, please provide valid time zone");

        RuleFor(x => x.ReferralCode)
           .Length(6).WithMessage("Referral code must be in 6 characters long")
           .When(x => !string.IsNullOrWhiteSpace(x.ReferralCode));

        RuleFor(e => e.NationalIdentity)
            .NotNull().WithMessage("Please fill the national identity")
            .SetValidator(new CreateNationalIdentityValidation());

        RuleFor(e => e.DrivingLicense)
            .NotNull().WithMessage("Please fill the driving license")
            .SetValidator(new CreateDrivingLicenseValidation());

        RuleFor(x => x.PersonalInfo)
            .NotNull().WithMessage("Personal info can not be null")
            .SetValidator(new CreatePersonalInfoValidation());

        When(x => x.Certifications is not null && x.Certifications.Any(), () =>
        {
            RuleForEach(e => e.Certifications).SetValidator(new CreateCertificationValidation());
        }); 
    }
     
}
