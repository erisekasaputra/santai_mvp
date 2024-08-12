using Account.API.Applications.Dtos.RequestDtos;
using FluentValidation;

namespace Account.API.Validations.MechanicUserValidations;

public class CreateMechanicUserValidation : AbstractValidator<MechanicUserRequestDto>
{
    public CreateMechanicUserValidation()
    {
        
    }
}
