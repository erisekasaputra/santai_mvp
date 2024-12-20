using Account.API.Applications.Dtos.RequestDtos; 
using Core.Validations;
using FluentValidation;

namespace Account.API.Validations.PersonalInfoValidations;

public class CreatePersonalInfoValidation : AbstractValidator<PersonalInfoRequestDto>
{
    public CreatePersonalInfoValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("The name must be between 2 and 50 characters long")
            .Must(NameValidation.IsValidName).WithMessage("The name must contain only alphabet, e.g: 'Michael'");

        RuleFor(x => x.MiddleName)
            .Length(2, 50).WithMessage("The name must be between 2 and 50 characters long")
            .Must(NameValidation.IsValidName).WithMessage("The name must contain only alphabet, e.g: 'John'")
            .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

        RuleFor(x => x.LastName)
            .Length(2, 50).WithMessage("The name must be between 2 and 50 characters long")
            .Must(NameValidation.IsValidName).WithMessage("The name must contain only alphabet, e.g: 'Doe'")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.DateOfBirth)
            .NotNull()
            .WithMessage("The birth date is required.")
            .NotEmpty()
            .WithMessage("The birth date is required.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is empty")
            .IsInEnum().WithMessage("Invalid gender."); 
    } 
}
