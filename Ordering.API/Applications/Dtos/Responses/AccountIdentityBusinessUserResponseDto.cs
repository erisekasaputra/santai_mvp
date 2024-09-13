namespace Ordering.API.Applications.Dtos.Responses;

public class AccountIdentityBusinessUserResponseDto
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string TimeZoneId { get; set; }
    public string BusinessName { get; set; }
    public string ContactPerson { get; set; }
    public string? TaxId { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? BusinessDescription { get; set; }
    public IEnumerable<AccountIdentityFleetResponseDto> Fleets { get; set; }
    public AccountIdentityBusinessUserResponseDto(
        Guid userId,
        string? email,
        string? phoneNumber,
        string timeZoneId,
        string businessName, 
        string contactPerson,
        string? taxId,
        string? websiteUrl,
        string? businessDescription,
        IEnumerable<AccountIdentityFleetResponseDto> fleets
        )
    {
        UserId = userId;
        Email = email;
        PhoneNumber = phoneNumber;
        TimeZoneId = timeZoneId;
        BusinessName = businessName;
        ContactPerson = contactPerson;
        TaxId = taxId;
        WebsiteUrl = websiteUrl;
        BusinessDescription = businessDescription;
        Fleets = fleets;
    }
}
