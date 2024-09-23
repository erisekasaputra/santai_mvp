namespace Account.API.Applications.Dtos.ResponseDtos;

public class GlobalUserResponseDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; } 
    public string TimeZoneId { get; set; }
    public string Fullname { get; set; }
    public IEnumerable<FleetResponseDto> Fleets { get; set; }
    public IEnumerable<Guid> UnknownFleets { get; set; } 

    public GlobalUserResponseDto(
        Guid id,
        string? email,
        string? phoneNumber,
        string timeZoneId,
        string fullname,
        IEnumerable<FleetResponseDto> fleets,
        IEnumerable<Guid> unknownFleets)
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
