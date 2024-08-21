using FluentValidation;
using Identity.API.Dto;

namespace Identity.API.Validations;

public class ForgotPasswordByEmailValidation : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordByEmailValidation()
    { 
        RuleFor(x => x.Identity)
            .NotEmpty().WithMessage("Email can not be null")
            .EmailAddress().WithMessage("Email address is invalid");
    }
}
