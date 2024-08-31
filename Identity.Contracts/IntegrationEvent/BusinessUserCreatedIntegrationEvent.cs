using Identity.Contracts.EventEntity;
using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public class BusinessUserCreatedIntegrationEvent : INotification
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; }
    public string TimeZoneId { get; set; }
    public string BusinessCode { get; set; }
    public string BusinessName { get; set; }
    public string ContactPerson { get; set; }
    public string? TaxId { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? BusinessDescription { get; set; }
    public string Password { get; set; }
    public IEnumerable<StaffEvent>? Staffs { get; set; }

    public BusinessUserCreatedIntegrationEvent(
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
        IEnumerable<StaffEvent>? staffs)
    {
        UserId = userId;
        Email = email;
        PhoneNumber = phoneNumber;
        TimeZoneId = timeZoneId;
        BusinessCode = businessCode;
        BusinessName = businessName;
        ContactPerson = contactPerson;
        TaxId = taxId;
        WebsiteUrl = websiteUrl;
        BusinessDescription = businessDescription;
        Password = password;    
        Staffs = staffs;
    }
}
