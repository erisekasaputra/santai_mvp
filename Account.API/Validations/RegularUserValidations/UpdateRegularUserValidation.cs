using Account.API.Applications.Commands.UpdateRegularUserByUserId;
using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using Account.API.Validations.PersonalInfoValidations;
using Account.Domain.ValueObjects;
using FluentValidation;

namespace Account.API.Validations.RegularUserValidations;

public class UpdateRegularUserValidation : AbstractValidator<UpdateRegularUserByUserIdCommand>
{
    public UpdateRegularUserValidation()
    {  
        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not empty")
            .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone can not empty")
            .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
            .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone is invalid, please provide valid time zone");

        RuleFor(x => x.PersonalInfo).SetValidator(new CreatePersonalInfoValidation());
    }
}
