namespace Ordering.API.Applications.Dtos.Responses;

public class AccountIdentityStaffUserResponseDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Name { get; set; }
    public string TimeZoneId { get; set; }
    public IEnumerable<AccountIdentityFleetResponseDto> Fleets { get; set; }

    public AccountIdentityStaffUserResponseDto(
        Guid id,
        string? email,
        string? phoneNumber,
        string name,
        string timeZoneId,
        IEnumerable<AccountIdentityFleetResponseDto> fleets)
    {
        Id = id;
        Email = email;
        PhoneNumber = phoneNumber;
        Name = name;
        TimeZoneId = timeZoneId;
        Fleets = fleets;
    }
}
