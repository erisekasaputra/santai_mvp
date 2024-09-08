using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class EmailRequestDto(string email)
{
    public string Email { get; set; } = email.CleanAndLowering(); 
}
