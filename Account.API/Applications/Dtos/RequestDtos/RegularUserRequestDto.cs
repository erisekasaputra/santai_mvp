using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class RegularUserRequestDto( 
    string email,
    string timeZoneId,
    string? referralCode,
    AddressRequestDto address,
    PersonalInfoRequestDto personalInfo)
{ 
    public required string Email { get; set; } = email.CleanAndLowering();
    public required string TimeZoneId { get; set; } = timeZoneId.Clean();
    public string? ReferralCode { get; set; } = referralCode?.Clean();
    public required AddressRequestDto Address { get; set; } = address;
    public required PersonalInfoRequestDto PersonalInfo { get; set; } = personalInfo;
}
