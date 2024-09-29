using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Core.Validations;
using FluentValidation;

namespace Account.API.Validations.NationalIdentityValidations;

public class CreateNationalIdentityValidation : AbstractValidator<NationalIdentityRequestDto>
{
    public CreateNationalIdentityValidation()
    {
        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("Identity number can not be null")
            .Length(3, 40).WithMessage("The identity number must be between 3 and 40 characters long");

        RuleFor(x => x.FrontSideImageUrl)
            .NotEmpty().WithMessage("Front side image url can not be empty");

        RuleFor(x => x.BackSideImageUrl)
            .NotEmpty().WithMessage("Back side image url can not be empty");
    }
}
