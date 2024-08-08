using Account.API.Applications.Dtos.RequestDtos; 
using FluentValidation;

namespace Account.API.Validations.UserValidation;

public class EmailRequestValidation : AbstractValidator<EmailRequestDto>
{
    public EmailRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email can not be empty")
            .EmailAddress().WithMessage("Email format is invalid");
    }
}
