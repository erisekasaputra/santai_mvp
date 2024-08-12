using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations; 
using Account.API.Validations.CertificationValidations;
using Account.API.Validations.DrivingLicenseValidations;
using Account.API.Validations.NationalIdentityValidations; 
using FluentValidation;

namespace Account.API.Validations.MechanicUserValidations;

public class CreateMechanicUserValidation : AbstractValidator<MechanicUserRequestDto>
{
    public CreateMechanicUserValidation()
    {
        RuleFor(x => x.IdentityId)
            .NotEmpty().WithMessage("Identity Id can not be null")
            .Must(MustValidGuid).WithMessage("Guid is not valid");
          
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username can not be empty")
            .Length(3, 20).WithMessage("The username must be between 3 and 20 characters long")
            .Must(UsernameExtension.IsValidUsername).WithMessage("The username must contain only lowercase and numbers");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Username can not be empty")
            .Length(5, 254).WithMessage("The username must be between 3 and 254 characters long")
            .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number can not be empty")
            .Length(3, 20).WithMessage("Phone number must be between 3 and 20 characters long")
            .Must(PhoneNumberExtension.IsValidPhoneNumber).WithMessage("Phone number can have number only");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
           .NotEmpty().WithMessage("Time zone can not be empty")
           .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
           .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone {PropertyValue} is invalid, please provide valid time zone");

        RuleFor(e => e.NationalIdentity)
            .NotNull().WithMessage("Please fill the national identity")
            .SetValidator(new CreateNationalIdentityValidation());

        RuleFor(e => e.DrivingLicense)
            .NotNull().WithMessage("Please fill the driving license")
            .SetValidator(new CreateDrivingLicenseValidation());

        When(x => x.Certifications is not null && x.Certifications.Any(), () =>
        {
            RuleForEach(e => e.Certifications).SetValidator(new CreateCertificationValidation());
        }); 
    }


    private bool MustValidGuid(Guid id)
    {
        return id != Guid.Empty;
    }
}
