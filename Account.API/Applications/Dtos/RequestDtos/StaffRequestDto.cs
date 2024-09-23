 
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
public class StaffRequestDto( 
    string phoneNumber, 
    string name,
    AddressRequestDto address,
    string timeZoneId,
    string password)
{ 
    public required string PhoneNumber { get; set; } = phoneNumber.Clean(); 
    public required string Name { get; set; } = name.Clean();
    public required AddressRequestDto Address { get; set; } = address;
    public required string TimeZoneId { get; set; } = timeZoneId.Clean();
    public required string Password { get; set; } = password.Clean();
}