 
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class EmailRequestDto(string email)
{
    public required string Email { get; set; } = email.CleanAndLowering(); 
}
