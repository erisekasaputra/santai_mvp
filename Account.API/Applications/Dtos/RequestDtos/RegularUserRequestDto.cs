using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class RegularUserRequestDto(
    Guid identityId,
    string username,
    string email,
    string phoneNumber,
    string timeZoneId,
    AddressRequestDto address,
    PersonalInfoRequestDto personalInfo,
    string deviceId)
{
    public Guid IdentityId { get; } = identityId;
    public string Username { get; } = username.CleanAndLowering();
    public string Email { get; set; } = email.CleanAndLowering();
    public string PhoneNumber { get; set; } = phoneNumber.Clean();
    public string TimeZoneId { get; } = timeZoneId.Clean();
    public AddressRequestDto Address { get; } = address;
    public PersonalInfoRequestDto PersonalInfo { get; } = personalInfo;
    public string DeviceId { get; } = deviceId.Clean();
}
