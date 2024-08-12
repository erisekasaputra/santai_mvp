using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.Validations.AddressValidations;
using FluentValidation;

namespace Account.API.Validations.BusinessUserValidations;

public class UpdateBusinessUserValidation : AbstractValidator<UpdateBusinessUserRequestDto>
{
    public UpdateBusinessUserValidation()
    { 
        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name can not be empty")
            .Length(3, 50).WithMessage("Business name must be between 3 and 50 characters long");

        RuleFor(x => x.ContactPerson)
            .NotEmpty().WithMessage("Contact person can not be empty")
            .Length(3, 254).WithMessage("Contact person must be between 3 and 254 characters long");

        RuleFor(x => x.TaxId)
            .Length(3, 50).WithMessage("Tax id must be between 3 and 50 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.TaxId));

        RuleFor(x => x.WebsiteUrl)
            .Length(3, 255).WithMessage("Website url must be between 3 and 255 characters long")
            .Must(UrlExtension.IsValidUrl).WithMessage("Website url format is invalid")
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));

        RuleFor(x => x.Description)
            .Length(1, 1000).WithMessage("Business description must be between 1 and 1000 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address can not be empty")
            .SetValidator(new AddressValidation());

        RuleFor(x => x.TimeZoneId)
           .NotEmpty().WithMessage("Time zone can not be empty")
           .Length(2, 40).WithMessage("Time zone must be between 2 and 40 characters long")
           .Must(DateTimeExtension.IsTimeZoneExists).WithMessage("Time zone {PropertyValue} is invalid, please provide valid time zone");
    }
}
