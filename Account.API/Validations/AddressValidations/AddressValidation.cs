using Account.API.Applications.Dtos.RequestDtos;
using FluentValidation;

namespace Account.API.Validations.AddressValidations;

public class AddressValidation : AbstractValidator<AddressRequestDto>
{
    public AddressValidation()
    { 
        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 is required.")
            .Length(3, 255).WithMessage("Address line 1 must be between 3 and 255 characters.");
         
        RuleFor(x => x.AddressLine2)
            .Must((address, addressLine2) =>
                string.IsNullOrEmpty(addressLine2) || !string.IsNullOrEmpty(address.AddressLine1))
            .WithMessage("Address line 2 cannot be filled unless address line 1 is filled.")
            .Length(3, 255).When(x => !string.IsNullOrEmpty(x.AddressLine2))
            .WithMessage("Address line 2 must be between 3 and 255 characters.");
         
        RuleFor(x => x.AddressLine3)
            .Must((address, addressLine3) =>
                string.IsNullOrEmpty(addressLine3) || !string.IsNullOrEmpty(address.AddressLine2))
            .WithMessage("Address line 3 cannot be filled unless address line 2 is filled.")
            .Length(3, 255).When(x => !string.IsNullOrEmpty(x.AddressLine3))
            .WithMessage("Address line 3 must be between 3 and 255 characters.");
         
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
