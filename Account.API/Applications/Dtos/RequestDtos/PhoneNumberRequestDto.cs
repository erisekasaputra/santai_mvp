using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class PhoneNumberRequestDto(string phoneNumber)
{
    public required string PhoneNumber { get; set; } = phoneNumber.Clean();
}
