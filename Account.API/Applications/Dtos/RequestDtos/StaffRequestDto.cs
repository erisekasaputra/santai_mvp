using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
public class StaffRequestDto(
    string Username,
    string PhoneNumber,
    string Email,
    string Name,
    AddressRequestDto Address,
    string TimeZoneId)
{
    public string Username { get; } = Username.CleanAndLowering();
    public string PhoneNumber { get; } = PhoneNumber.Clean();
    public string Email { get; } = Email.CleanAndLowering();
    public string Name { get; } = Name.Clean();
    public AddressRequestDto Address { get; } = Address;
    public string TimeZoneId { get; } = TimeZoneId.Clean();
}