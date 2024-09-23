 
namespace Ordering.API.Applications.Dtos.Responses;

public class AccountIdentityResponseDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string TimeZoneId { get; set; } 
    public string Fullname { get; set; }
    public IEnumerable<AccountIdentityFleetResponseDto> Fleets { get; set; }
    public IEnumerable<Guid> UnknownFleets { get; set; }  
    public AccountIdentityResponseDto(
        Guid id,
        string? email,
        string? phoneNumber,
        string timeZoneId,
        string fullname,    
        IEnumerable<AccountIdentityFleetResponseDto> fleets,
        IEnumerable<Guid> unknownFleets )
    {  
        Id = id;
        Email = email;
        PhoneNumber = phoneNumber;
        TimeZoneId = timeZoneId;
        Fullname = fullname;
        Fleets = fleets;
        UnknownFleets = unknownFleets; 
    }
}
