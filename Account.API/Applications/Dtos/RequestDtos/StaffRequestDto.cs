using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
public class StaffRequestDto( 
    string phoneNumber,
    string? email,
    string name,
    AddressRequestDto address,
    string timeZoneId,
    string password)
{ 
    public string PhoneNumber { get; set; } = phoneNumber.Clean();
    public string? Email { get; set; } = email?.CleanAndLowering();
    public string Name { get; } = name.Clean();
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; set; } = timeZoneId.Clean();
    public string Password { get; set; } = password.Clean();
}