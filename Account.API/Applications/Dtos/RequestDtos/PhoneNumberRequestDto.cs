using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class PhoneNumberRequestDto(string phoneNumber)
{
    public string PhoneNumber { get; set; } = phoneNumber.Clean();
}
