using Account.API.Applications.Dtos.RequestDtos;
using FluentValidation;

namespace Account.API.Validations.BusinessLicenseValidations;

public class BusinessLicenseValidation : AbstractValidator<BusinessLicenseRequestDto>
{
	public BusinessLicenseValidation()
	{
		RuleFor(x => x.Name)
            .NotEmpty().WithMessage("License name can not empty")
            .Length(3, 50).WithMessage("License name must be between 3 and 50 characters long");

        RuleFor(x => x.LicenseNumber)
			.NotEmpty().WithMessage("License number can not empty")
			.Length(3, 100).WithMessage("License number must be between 3 and 100 characters long");
		
		RuleFor(x => x.Description)
            .NotEmpty().WithMessage("License description can not empty")
            .Length(1, 1000).WithMessage("License number must be between 1 and 1000 characters long");
    }
}
