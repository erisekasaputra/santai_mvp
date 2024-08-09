using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Events;
using Account.Domain.Exceptions;
using Account.Domain.Extensions;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public class BusinessUser : User
{  
    public string Code { get; private init; }  
    public string BusinessName { get; private set; } 
    public string ContactPerson { get; private set; } 
    public string? TaxId { get; private set; } 
    public string? WebsiteUrl { get; private set; } 
    public string? Description { get; private set; }    
    public ICollection<Staff>? Staffs { get; private set; } 
    public ICollection<BusinessLicense>? BusinessLicenses { get; private set; }  
    public BusinessUser() : base()
    {
        Code = string.Empty;
        BusinessName = string.Empty;
        ContactPerson = string.Empty;
    }

    public BusinessUser(
        Guid identityId,
        string username,
        string email,
        string phoneNumber,
        Address address,
        string businessName,
        string? taxId,
        string contactPerson,
        string? websiteUrl,
        string? description, 
        string timeZoneId) : base(identityId, username, email, phoneNumber, address, timeZoneId)
    { 
        Code = UniqueIdGenerator.Generate(Id);
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        TaxId = taxId;
        ContactPerson = contactPerson ?? throw new ArgumentNullException(nameof(contactPerson));  
        WebsiteUrl = websiteUrl;
        Description = description;

        RaiseBusinessUserCreatedDomainEvent(this);
    } 

    public void Update(string businessName, string contactPerson, string? taxId, string? websiteUrl, string? description, Address address, string timeZoneId)
    {
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        ContactPerson = contactPerson ?? throw new ArgumentNullException(nameof(contactPerson));
        TaxId = taxId;
        WebsiteUrl = websiteUrl;
        Description = description;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));

        RaiseBusinessUserUpdatedDomainEvent(Id, businessName, contactPerson, taxId, websiteUrl, description, address, timeZoneId);
    }
     
    public override void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    { 
        base.AddReferralProgram(referralRewardPoint, referralValidDate); 
        // domain event already published in the parent class
    }

    public (BusinessLicense? newBusinessLicense, string? ErrorParameter, string? ErrorMessage) AddBusinessLicenses(string licenseNumber, string name, string description)
    {  
        var licenses = BusinessLicenses?.SingleOrDefault(c => c.LicenseNumber
            .Equals(licenseNumber, StringComparison.CurrentCultureIgnoreCase) && c.VerificationStatus == Enumerations.VerificationState.Accepted);

        if (licenses is not null)
        {
            return (null, "LicenseNumber", $"Business license with number {licenseNumber} and status 'Accepted' already registered");
        } 

        var certification = new BusinessLicense(Id, licenseNumber, name, description);
        
        BusinessLicenses?.Add(certification);

        RaiseBusinessLicenseAddedDomainEvent(certification);

        return (certification, null, null);
    }
    
    public BusinessLicense? RemoveBusinessLicenses(Guid id)
    {
        if (BusinessLicenses is null)
        {
            return null;
        }

        var certification = (BusinessLicenses?.SingleOrDefault(e => e.Id == id)) ?? throw new DomainException($"Business license with id {id} does not exists in the current aggregate");
        
        BusinessLicenses?.Remove(certification); 
        RaiseBusinessLicenseRemovedDomainEvent(certification);
        return certification;
    } 

    public (Staff? newStaff, string? ErrorParameter, string? ErrorMessage) AddStaff(string username, string email, string phoneNumber, string name, Address address, string timeZoneId)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(timeZoneId);
        ArgumentNullException.ThrowIfNull(address); 
         
        Staffs ??= [];

        if (Staffs.Any(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase)))
        {

            return (null, "Username", $"Username: {username} is already registered. Please use a different username.");
        } 

        if (Staffs.Any(x => x.PhoneNumber.Equals(phoneNumber, StringComparison.CurrentCultureIgnoreCase)
            || (x.NewPhoneNumber?.Equals(phoneNumber, StringComparison.CurrentCultureIgnoreCase)) == true))
        {
            return (null, "PhoneNumber", $"Phone Number: {phoneNumber} is already registered. Please use a different phone number."); 
        }

        if (Staffs.Any(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)
            || (x.NewEmail?.Equals(email, StringComparison.CurrentCultureIgnoreCase) == true)))
        {
            return (null, "Email", $"Email: {email} is already registered. Please use a different email."); 
        } 

        var staff = new Staff(Id, Code, username, email, phoneNumber, name, address, timeZoneId, null); 
        Staffs.Add(staff); 
        RaiseStaffAddedDomainEvent(staff); 
        return (staff, null, null);
    } 

    public Staff? RemoveStaff(Guid id)
    {
        if (Staffs is null)
        {
            return null;
        }

        var staff = Staffs?.FirstOrDefault(x => x.Id == id);

        if (staff is null)
        {
            return null;
        }

        Staffs!.Remove(staff);
        RaiseStaffRemovedDomainEvent(staff);
        return staff;
    }

    public void Delete()
    {
        RaiseBusinessUserDeletedDomainEvent();
    }

    private void RaiseBusinessUserDeletedDomainEvent()
    {
        AddDomainEvent(new BusinessUserDeletedDomainEvent(this));
    }

    private void RaiseBusinessUserUpdatedDomainEvent(Guid id, string businessName, string contactPerson, string? taxId, string? websiteUrl, string? description, Address address, string timeZoneId)
    {
        AddDomainEvent(new BusinessUserUpdatedDomainEvent(
            id,
            businessName,
            contactPerson,
            taxId,
            websiteUrl,
            description,
            address,
            timeZoneId));
    }

    private void RaiseBusinessLicenseRemovedDomainEvent(BusinessLicense license)
    {
        AddDomainEvent(new BusinessLicenseRemovedDomainEvent(license));
    }

    private void RaiseBusinessLicenseAddedDomainEvent(BusinessLicense license)
    {
        AddDomainEvent(new BusinessLicenseAddedDomainEvent(license));
    }

    private void RaiseStaffAddedDomainEvent(Staff staff)
    {
        AddDomainEvent(new StaffAddedDomainEvent(staff));
    }

    private void RaiseStaffRemovedDomainEvent(Staff staff)
    {
        AddDomainEvent(new StaffRemovedDomainEvent(staff));
    }

    private void RaiseBusinessUserCreatedDomainEvent(BusinessUser user)
    {
        AddDomainEvent(new BusinessUserCreatedDomainEvent(user));
    }
}
