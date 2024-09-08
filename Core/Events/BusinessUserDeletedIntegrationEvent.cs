using MediatR;

namespace Core.Events;

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
   IEnumerable<StaffIntegrationEvent>? staffs) : INotification
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
    public IEnumerable<StaffIntegrationEvent>? Staffs { get; set; } = staffs;
}
