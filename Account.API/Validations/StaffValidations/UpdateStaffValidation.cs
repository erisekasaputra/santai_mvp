using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using FluentValidation;

namespace Account.API.Validations.StaffValidations;

public class UpdateStaffValidation : AbstractValidator<UpdateStaffRequestDto>
{
    public UpdateStaffValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name can not be empty")
            .Length(3, 50).WithMessage("The name must be between 3 and 50 characters long")
            .Must(NameExtension.IsValidName).WithMessage("The name must contain only alphabet and can not have multiple spaces (only single space on each separated name), e.g: 'Michael John Doe'");

        RuleFor(x => x.TimeZoneId)
                   .NotEmpty().WithMessage("Time zone can not be empty")
                   .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
                   .Must(DateTimeExtension.IsTimeZoneExists);

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation());
    }
}
