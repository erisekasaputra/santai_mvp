using Account.API.Applications.Dtos.RequestDtos;
using FluentValidation;

namespace Account.API.Validations.AddressValidations;

public class AddressValidation : AbstractValidator<AddressRequestDto>
{
    public AddressValidation()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 can not empty")
            .Length(3, 255).WithMessage("Address line 1 must be between 3 and 255 characters long");

        RuleFor(x => x.AddressLine2)
            .Length(3, 255).WithMessage("Address line 2 must be between 3 and 255 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.AddressLine2));

        RuleFor(x => x.AddressLine3)
            .Length(3, 255).WithMessage("Address line 3 must be between 3 and 255 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.AddressLine3))
            .When(x => !string.IsNullOrWhiteSpace(x.AddressLine2)).WithMessage("Please complete address line 2 before filling in address line 3.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City can not empty")
            .Length(3, 50).WithMessage("City must be between 3 and 50 characters long");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State can not empty")
            .Length(3, 50).WithMessage("State must be between 3 and 50 characters long");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code can not empty")
            .Length(3, 20).WithMessage("Postal code must be between 3 and 20 characters long");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country can not empty")
            .Length(3, 50).WithMessage("Country must be between 3 and 50 characters long");
    }
}
