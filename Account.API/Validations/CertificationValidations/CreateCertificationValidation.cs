using Account.API.Applications.Dtos.RequestDtos; 
using FluentValidation;

namespace Account.API.Validations.CertificationValidations;

public class CreateCertificationValidation : AbstractValidator<CertificationRequestDto>
{
    public CreateCertificationValidation()
    {
        RuleFor(x => x.CertificationId)
            .NotEmpty().WithMessage("Certification id can not be empty")
            .Length(3, 255).WithMessage("The certificaton id must be between 3 and 255 characters long");

        RuleFor(x => x.CertificationName)
           .NotEmpty().WithMessage("Certification name can not be empty")
           .Length(3, 100).WithMessage("The certification name must be between 3 and 100 characters long");

        RuleFor(x => x.ValidDate)
            .NotEmpty().WithMessage("Valid date is required.")
            .Must(date => date > DateTime.MinValue && date != default).WithMessage("Valid date is invalid.");

        RuleFor(x => x.Specialization)
           .NotEmpty().When(x => x.Specialization != null).WithMessage("Specialization list cannot be empty.")
           .Must(specializations => specializations != null && specializations.Any())
               .When(x => x.Specialization != null).WithMessage("Specialization list cannot be empty.")
           .ForEach(specialization =>
           {
               specialization
                   .NotEmpty().WithMessage("Specialization cannot be empty.")
                   .MaximumLength(50).WithMessage("Specialization cannot exceed 50 characters.");
           })
           .When(x => x.Specialization != null);
    }
}
