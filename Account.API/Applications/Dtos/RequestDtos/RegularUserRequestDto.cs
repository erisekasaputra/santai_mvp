using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class RegularUserRequestDto(
    Guid IdentityId,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressRequestDto Address,
    PersonalInfoRequestDto PersonalInfo,
    string DeviceId)
{
    public Guid IdentityId { get; } = IdentityId;
    public string Username { get; } = Username.CleanAndLowering();
    public string Email { get; } = Email.CleanAndLowering();
    public string PhoneNumber { get; } = PhoneNumber.Clean();
    public string TimeZoneId { get; } = TimeZoneId.Clean();
    public AddressRequestDto Address { get; } = Address;
    public PersonalInfoRequestDto PersonalInfo { get; } = PersonalInfo;
    public string DeviceId { get; } = DeviceId.Clean();
}
