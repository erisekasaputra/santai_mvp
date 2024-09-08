using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Core.Validations;
using FluentValidation;

namespace Account.API.Validations.UserValidation;

public class PhoneNumbeRequestValidation : AbstractValidator<PhoneNumberRequestDto>
{
    public PhoneNumbeRequestValidation()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number can not be empty")
            .Must(PhoneNumberValidation.IsValidPhoneNumber).WithMessage("Phone number must consist of number only and start with '+'");
    }
}
