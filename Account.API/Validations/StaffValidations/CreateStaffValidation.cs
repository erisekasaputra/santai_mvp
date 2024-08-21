using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using FluentValidation;

namespace Account.API.Validations.StaffValidations;

public class CreateStaffValidation : AbstractValidator<StaffRequestDto>
{
    public CreateStaffValidation()
    { 
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name can not be empty")
            .Length(3, 50).WithMessage("The name must be between 3 and 50 characters long")
            .Must(NameExtension.IsValidName).WithMessage("The name must contain only alphabet and can not have multiple spaces (only single space on each separated name), e.g: 'Michael John Doe'");

        RuleFor(x => x.Email)
           .Length(3, 254).WithMessage("The email must be between 3 and 254 characters long")
           .EmailAddress().WithMessage("Email format is invalid")
           .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number can not be empty")
            .Length(3, 20).WithMessage("Phone number must be between 3 and 20 characters long")
            .Must(PhoneNumberExtension.IsValidPhoneNumber).WithMessage("Phone number must consist of number only and start with '+'");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone can not be empty")
            .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
            .Must(DateTimeExtension.IsTimeZoneExists);  
        
        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation()); 

    }
}
