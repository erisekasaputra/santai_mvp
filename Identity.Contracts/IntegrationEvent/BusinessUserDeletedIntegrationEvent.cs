using Identity.Contracts.EventEntity;
using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public class BusinessUserDeletedIntegrationEvent(
   Guid userId,
   string? email,
   string phoneNumber,
   string timeZoneId,
   string businessCode,
   string businessName,
   string contactPerson,
   string? taxId,
   string? websiteUrl,
   string? businessDescription,
   string password,
   IEnumerable<StaffEvent>? staffs) : INotification
{
    public Guid UserId { get; set; } = userId;
    public string? Email { get; set; } = email;
    public string PhoneNumber { get; set; } = phoneNumber;
    public string TimeZoneId { get; set; } = timeZoneId;
    public string BusinessCode { get; set; } = businessCode;
    public string BusinessName { get; set; } = businessName;
    public string ContactPerson { get; set; } = contactPerson;
    public string? TaxId { get; set; } = taxId;
    public string? WebsiteUrl { get; set; } = websiteUrl;
    public string? BusinessDescription { get; set; } = businessDescription;
    public string Password { get; set; } = password;
    public IEnumerable<StaffEvent>? Staffs { get; set; } = staffs;
}
