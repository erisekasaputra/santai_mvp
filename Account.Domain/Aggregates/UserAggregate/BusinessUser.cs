using Account.Domain.Aggregates.BusinessLicenseAggregate; 
using Account.Domain.Enumerations;
using Account.Domain.Events; 
using Account.Domain.Extensions;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public class BusinessUser : User
{
    public string Code { get; private init; }
    public string BusinessName { get; private set; }
    public string EncryptedContactPerson { get; private set; }
    public string? EncryptedTaxId { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Description { get; private set; }
    public ICollection<Staff>? Staffs { get; private set; }
    public ICollection<BusinessLicense>? BusinessLicenses { get; private set; } 
    protected BusinessUser() : base()
    { 
    }

    public BusinessUser(
        Guid identityId,
        string username,
        string email,
        string encryptedEmail,
        string phoneNumber,
        string encryptedPhoneNumber,
        Address address,
        string businessName,
        string? encryptedTaxId,
        string encryptedContactPerson,
        string? websiteUrl,
        string? description, 
        string timeZoneId) : base(identityId, username, email, encryptedEmail, phoneNumber, encryptedPhoneNumber, address, timeZoneId)
    { 
        Code = UniqueIdGenerator.Generate(Id);
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        EncryptedTaxId = encryptedTaxId;
        EncryptedContactPerson = encryptedContactPerson ?? throw new ArgumentNullException(nameof(encryptedContactPerson));  
        WebsiteUrl = websiteUrl;
        Description = description; 
        RaiseBusinessUserCreatedDomainEvent(this);
    } 

    public void Update(string businessName, string encryptedContactPerson, string? encryptedTaxId, string? websiteUrl, string? description, Address address, string timeZoneId)
    {
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        EncryptedContactPerson = encryptedContactPerson ?? throw new ArgumentNullException(nameof(encryptedContactPerson));
        EncryptedTaxId = encryptedTaxId;
        WebsiteUrl = websiteUrl;
        Description = description;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
        UpdatedAtUtc = DateTime.UtcNow;
        RaiseBusinessUserUpdatedDomainEvent(Id, businessName, encryptedContactPerson, encryptedTaxId, websiteUrl, description, address, timeZoneId);
    }
     
    public override void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    { 
        // domain event already published in the parent class
        base.AddReferralProgram(referralRewardPoint, referralValidDate); 
    }

    public (BusinessLicense? newBusinessLicense, string? ErrorParameter, string? ErrorMessage) AddBusinessLicenses(string hashedLicenseNumber, string encryptedLicenseNumber, string name, string description)
    {
        BusinessLicenses ??= [];

        var licenses = BusinessLicenses?.SingleOrDefault(c => c.HashedLicenseNumber == hashedLicenseNumber && c.VerificationStatus == VerificationState.Accepted);

        if (licenses is not null)
        {
            return (null, "BusinessLicense.LicenseNumber", $"Business license already registered");
        } 

        var license = new BusinessLicense(
            Id,
            hashedLicenseNumber,
            encryptedLicenseNumber,
            name,
            description);
        
        BusinessLicenses?.Add(license);

        RaiseBusinessLicenseAddedDomainEvent(license);

        return (license, null, null);
    }

    public (BusinessLicense? newBusinessLicense, string? ErrorParameter, string? ErrorMessage) AddBusinessLicenses(BusinessLicense businessLicense)
    {
        BusinessLicenses ??= [];

        var licenses = BusinessLicenses?.SingleOrDefault(c => c.HashedLicenseNumber == businessLicense.HashedLicenseNumber && c.VerificationStatus == VerificationState.Accepted);

        if (licenses is not null)
        {
            return (null, "BusinessLicense.LicenseNumber", $"Business license already registered");
        }

        var license = new BusinessLicense(
            Id,
            businessLicense.HashedLicenseNumber,
            businessLicense.EncryptedLicenseNumber,
            businessLicense.Name,
            businessLicense.Description);

        BusinessLicenses?.Add(license); 
        
        RaiseBusinessLicenseAddedDomainEvent(license); 
        
        return (license, null, null);
    } 

    public (Staff? newStaff, string? ErrorParameter, string? ErrorMessage) AddStaff(
        string username,
        string hashedEmail,
        string encryptedEmail,
        string hashedPhoneNumber,
        string encryptedPhoneNumber,
        string name,
        Address address,
        string timeZoneId)
    { 
        ArgumentNullException.ThrowIfNull(address);  
         
        Staffs ??= [];

        if (Staffs.Any(x => x.Username == username))
        { 
            return (null, "Staff.Username", $"Username is already registered");
        } 

        if (Staffs.Any(x => x.HashedPhoneNumber == hashedPhoneNumber || x.NewHashedPhoneNumber == hashedPhoneNumber))
        {
            return (null, "Staff.PhoneNumber", $"Phone number is already registered"); 
        }

        if (Staffs.Any(x => x.HashedEmail == hashedEmail || x.NewHashedEmail == hashedEmail))
        {
            return (null, "Staff.Email", "Email is already registered"); 
        } 

        
        var staff = new Staff(
            Id,
            Code,
            username,
            hashedEmail,
            encryptedEmail,
            hashedPhoneNumber,
            encryptedPhoneNumber,
            name,
            address,
            timeZoneId,
            null); 

        Staffs.Add(staff); 

        RaiseStaffAddedDomainEvent(staff); 
        
        return (staff, null, null);
    }

    public (Staff? newStaff, string? ErrorParameter, string? ErrorMessage) AddStaff(Staff staff)
    { 
        ArgumentNullException.ThrowIfNull(staff.Address);

        Staffs ??= [];

        if (Staffs.Any(x => x.Username == staff.Username))
        {
            return (null, "Staff.Username", $"Username is already registered");
        }

        if (Staffs.Any(x => x.HashedPhoneNumber == staff.HashedPhoneNumber || x.NewHashedPhoneNumber == staff.HashedPhoneNumber))
        {
            return (null, "Staff.PhoneNumber", $"Phone number is already registered");
        }

        if (Staffs.Any(x => x.HashedEmail == staff.HashedEmail || x.NewHashedEmail == staff.HashedEmail))
        {
            return (null, "Staff.Email", "Email is already registered");
        }

        Staffs.Add(staff);

        RaiseStaffAddedDomainEvent(staff);
        
        return (staff, null, null);
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
     
    private void RaiseBusinessLicenseAddedDomainEvent(BusinessLicense license)
    {
        AddDomainEvent(new BusinessLicenseAddedDomainEvent(license));
    }

    private void RaiseStaffAddedDomainEvent(Staff staff)
    {
        AddDomainEvent(new StaffAddedDomainEvent(staff));
    } 
    private void RaiseBusinessUserCreatedDomainEvent(BusinessUser user)
    {
        AddDomainEvent(new BusinessUserCreatedDomainEvent(user));
    }
}
