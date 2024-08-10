using Account.API.Applications.Dtos.RequestDtos;
using FluentValidation;

namespace Account.API.Validations.UserValidation;

public class DeviceIdRequestValidation : AbstractValidator<DeviceIdRequestDto>
{
    public DeviceIdRequestValidation()
    {
        RuleFor(x => x.DeviceId) 
           .NotEmpty().WithMessage("Device id can not empty")
           .Length(1, 255).WithMessage("The device must be between 1 and 255 characters long");
    }
}
