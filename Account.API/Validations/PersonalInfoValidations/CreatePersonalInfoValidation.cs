using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using FluentValidation;

namespace Account.API.Validations.PersonalInfoValidations;

public class CreatePersonalInfoValidation : AbstractValidator<PersonalInfoRequestDto>
{
    public CreatePersonalInfoValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("The name must be between 2 and 50 characters long")
            .Must(NameExtension.IsValidName).WithMessage("The name must contain only alphabet, e.g: 'Michael'");

        RuleFor(x => x.MiddleName)
            .Length(2, 50).WithMessage("The name must be between 2 and 50 characters long")
            .Must(NameExtension.IsValidName).WithMessage("The name must contain only alphabet, e.g: 'John'")
            .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

        RuleFor(x => x.LastName)
            .Length(2, 50).WithMessage("The name must be between 2 and 50 characters long")
            .Must(NameExtension.IsValidName).WithMessage("The name must contain only alphabet, e.g: 'Doe'")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName)); 

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender.");

        RuleFor(p => p.ProfilePicture)
            .MaximumLength(255).WithMessage("URL path is too long; it must not exceed 255 characters.")
            .Must(UrlExtension.IsValidImageUrl).WithMessage("Profile picture URL must be a valid image URL (jpg, jpeg, png).")
            .When(x => !string.IsNullOrWhiteSpace(x.ProfilePicture));
    } 
}
