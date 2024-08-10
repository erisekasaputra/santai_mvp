using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using Account.API.Validations.PersonalInfoValidations;
using FluentValidation;

namespace Account.API.Validations.RegularUserValidations;

public class CreateRegularUserValidation : AbstractValidator<RegularUserRequestDto>
{
    public CreateRegularUserValidation()
    {   
        RuleFor(x => x.Username)
           .NotEmpty().WithMessage("Username can not empty")
           .Length(3, 20).WithMessage("The username must be between 3 and 20 characters long")
           .Must(UsernameExtension.IsValidUsername).WithMessage("The username must contain only lowercase and numbers");

        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Username can not empty")
           .Length(5, 254).WithMessage("The username must be between 3 and 254 characters long")
           .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number can not empty")
            .Length(3, 20).WithMessage("Phone number must be between 3 and 20 characters long")
            .Must(PhoneNumberExtension.IsValidPhoneNumber).WithMessage("Phone number can have number only");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone can not empty")
            .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
            .Must(DateTimeExtension.IsTimeZoneExists);

        RuleFor(x => x.Address)
           .NotNull().WithMessage("Address can not empty")
           .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone can not empty")
            .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
            .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone is invalid, please provide valid time zone");

        RuleFor(x => x.DeviceId)
           .NotEmpty().WithMessage("Device id can not empty")
           .Length(1, 255).WithMessage("The device must be between 1 and 255 characters long");

        RuleFor(x => x.PersonalInfo).SetValidator(new CreatePersonalInfoValidation());
    }
}
