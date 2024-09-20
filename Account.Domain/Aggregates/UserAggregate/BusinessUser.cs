using Account.Domain.Aggregates.BusinessLicenseAggregate; 
using Account.Domain.Enumerations;
using Account.Domain.Events; 
using Account.Domain.Extensions;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public class BusinessUser : BaseUser
{
    public string Code { get; private init; }
    public string BusinessName { get; private set; }
    public string EncryptedContactPerson { get; private set; }
    public string? EncryptedTaxId { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Description { get; private set; }
    public string Password { get; private set; }
    public ICollection<Staff>? Staffs { get; private set; }
    public ICollection<BusinessLicense>? BusinessLicenses { get; private set; } 
    protected BusinessUser() : base()
    {
        Password = string.Empty;
        Code = null!;
        BusinessName = null!;
        EncryptedContactPerson = null!;
    }

    public BusinessUser(   
        string phoneNumber,
        string encryptedPhoneNumber,
        Address address,
        string businessName,
        string? encryptedTaxId,
        string encryptedContactPerson,
        string? websiteUrl,
        string? description, 
        string timeZoneId,
        string password) : base(
            businessName, 
            null, 
            null, 
            phoneNumber, 
            encryptedPhoneNumber, 
            address, 
            timeZoneId, 
            string.Empty,
            isEmailVerified: false, 
            isPhoneNumberVerified: true)
    {  
        Code = UniqueIdGenerator.Generate(Id);
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        EncryptedTaxId = encryptedTaxId;
        EncryptedContactPerson = encryptedContactPerson ?? throw new ArgumentNullException(nameof(encryptedContactPerson));  
        WebsiteUrl = websiteUrl;
        Description = description;
        Password = password;
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
        string hashedPhoneNumber,
        string encryptedPhoneNumber,
        string name,
        Address address,
        string timeZoneId,
        string password)
    { 
        ArgumentNullException.ThrowIfNull(address);  
         
        Staffs ??= []; 

        if (Staffs.Any(x => x.HashedPhoneNumber == hashedPhoneNumber || x.NewHashedPhoneNumber == hashedPhoneNumber))
        {
            return (null, "Staff.PhoneNumber", $"Phone number is already registered"); 
        }  
        
        var staff = new Staff(
            Id,
            Code, 
            hashedPhoneNumber,
            encryptedPhoneNumber,
            name,
            address,
            timeZoneId, 
            password,
            raiseCreatedEvent: false); 

        Staffs.Add(staff);
        
        return (staff, null, null);
    }

    public (Staff? newStaff, string? ErrorParameter, string? ErrorMessage) AddStaff(Staff staff)
    { 
        ArgumentNullException.ThrowIfNull(staff.Address);

        Staffs ??= []; 

        if (Staffs.Any(x => x.HashedPhoneNumber == staff.HashedPhoneNumber || x.NewHashedPhoneNumber == staff.HashedPhoneNumber))
        {
            return (null, "Staff.PhoneNumber", $"Phone number is already registered");
        }

        if (Staffs.Any(x => x.HashedEmail == staff.HashedEmail || x.NewHashedEmail == staff.HashedEmail))
        {
            return (null, "Staff.Email", "Email is already registered");
        }

        Staffs.Add(staff); 
        
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

    private void RaiseBusinessUserCreatedDomainEvent(BusinessUser user)
    {
        AddDomainEvent(new BusinessUserCreatedDomainEvent(user));
    }
}
