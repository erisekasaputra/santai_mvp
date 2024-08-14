using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using Account.API.Validations.PersonalInfoValidations;
using FluentValidation;

namespace Account.API.Validations.MechanicUserValidations;

public class UpdateMechanicUserValidation : AbstractValidator<UpdateMechanicRequestDto>
{
    public UpdateMechanicUserValidation()
    { 
        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation());

        RuleFor(x => x.PersonalInfo)
           .NotNull().WithMessage("Personal info can not be null")
           .SetValidator(new CreatePersonalInfoValidation());

        RuleFor(x => x.TimeZoneId)
           .NotEmpty().WithMessage("Time zone can not be empty")
           .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
           .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone {PropertyValue} is invalid, please provide valid time zone"); 
    }
}
