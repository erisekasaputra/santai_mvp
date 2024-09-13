 
namespace Ordering.API.Applications.Dtos.Responses;

public class AccountIdentityRegularUserResponseDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string TimeZoneId { get; set; } 
    public AccountIdentityPersonalInfoResponseDto PersonalInfo { get; set; }
    public IEnumerable<AccountIdentityFleetResponseDto> Fleets { get; set; }

    public AccountIdentityRegularUserResponseDto(
        Guid id,
        string? email,
        string? phoneNumber,
        string timeZoneId,
        AccountIdentityPersonalInfoResponseDto personalInfo,
        IEnumerable<AccountIdentityFleetResponseDto> fleets
        )
    { 
        Id = id;
        Email = email;
        PhoneNumber = phoneNumber;
        TimeZoneId = timeZoneId;
        PersonalInfo = personalInfo;
        Fleets = fleets;
    }
}
