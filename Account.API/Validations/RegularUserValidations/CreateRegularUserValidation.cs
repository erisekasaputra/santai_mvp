using Account.API.Applications.Dtos.RequestDtos; 
using Account.API.Validations.AddressValidations;
using Account.API.Validations.PersonalInfoValidations;
using Core.Extensions;
using FluentValidation;

namespace Account.API.Validations.RegularUserValidations;

public class CreateRegularUserValidation : AbstractValidator<RegularUserRequestDto>
{
    public CreateRegularUserValidation()
    {   
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email can not be empty")
            .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone can not be empty")
            .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
            .Must(DateTimeExtension.IsTimeZoneExists);

        RuleFor(x => x.Address)
           .NotNull().WithMessage("Address can not be empty")
           .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone can not be empty")
            .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
            .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone is invalid, please provide valid time zone");

        RuleFor(x => x.ReferralCode)
            .Length(6).WithMessage("Referral code must be in 6 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.ReferralCode)); 
 
        RuleFor(x => x.PersonalInfo).SetValidator(new CreatePersonalInfoValidation());
    } 
}
