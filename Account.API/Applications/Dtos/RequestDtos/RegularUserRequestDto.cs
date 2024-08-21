using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class RegularUserRequestDto(
    string timeZoneId,
    string? referralCode,
    AddressRequestDto address,
    PersonalInfoRequestDto personalInfo,
    string deviceId)
{  
    public string TimeZoneId { get; } = timeZoneId.Clean();
    public string? ReferralCode { get; } = referralCode?.Clean();
    public AddressRequestDto Address { get; } = address;
    public PersonalInfoRequestDto PersonalInfo { get; } = personalInfo;
    public string DeviceId { get; } = deviceId.Clean();
}
