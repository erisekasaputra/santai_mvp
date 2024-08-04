using Account.Domain.Aggregates.BusinessLicenseAggregate; 
using Account.Domain.Aggregates.LoyaltyAggregate;
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

    public ReferralProgram ReferralProgram { get; private set; }

    public LoyaltyProgram LoyaltyProgram { get; private set; }   

    public ICollection<Staff>? Staffs { get; private set; }

    private List<BusinessLicense>? _businessLicenses { get; set; }

    public IReadOnlyCollection<BusinessLicense>? BusinessLicenses => _businessLicenses?.AsReadOnly();

    public BusinessUser() : base()
    {
        _businessLicenses = [];
        Staffs = [];
    }

    public BusinessUser(
        string username,
        string email,
        string phoneNumber,
        Address address,
        string businessName,
        string? taxId,
        string contactPerson,
        string? websiteUrl,
        string? description,
        int referralRewardPoint) : base(username, email, phoneNumber, address)
    { 
        Code = UniqueIdGenerator.Generate(Id);
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        TaxId = taxId;
        ContactPerson = contactPerson ?? throw new ArgumentNullException(nameof(contactPerson));  
        WebsiteUrl = websiteUrl;
        Description = description;
        ReferralProgram = new ReferralProgram(Id, DateTime.UtcNow, referralRewardPoint);
        LoyaltyProgram = new LoyaltyProgram(Id, 0, Enumerations.LoyaltyTier.Basic); 
    }

    public void AddBusinessLicenses(string licenseNumber, string name, string description)
    {
        var businessLicenses = BusinessLicenses?.SingleOrDefault(c => c.LicenseNumber == licenseNumber);

        if (businessLicenses is not null)
        {
            throw new DomainException($"Business license with number {licenseNumber} is already registered");
        }

        _businessLicenses ??= [];
        _businessLicenses.Add(new BusinessLicense(Id, licenseNumber, name, description));
    }
    
    public void RemoveBusinessLicenses(Guid id)
    {
        if (_businessLicenses is null)
        {
            return;
        }

        var certification = BusinessLicenses?.SingleOrDefault(e => e.Id == id);

        if (certification is not null)
        {
            _businessLicenses.Remove(certification);
        }
    } 

    public void AddStaff(string username, string email, string phoneNumber, string name)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
         
        Staffs ??= [];

        if (Staffs.Any(x => x.PhoneNumber.Trim().Equals(phoneNumber.Trim(), StringComparison.CurrentCultureIgnoreCase)))
        {
            throw new DomainException($"Phone number {phoneNumber} is already registered. Please use a different phone number.");
        }

        if(Staffs.Any(x => x.Email.Trim().Equals(email.Trim(), StringComparison.CurrentCultureIgnoreCase)))
        {
            throw new DomainException($"Email {email} is already registered. Please use a different email.");
        }

        var staff = new Staff(Id, Code, username, email, phoneNumber, name, null);

        Staffs.Add(staff);
    }

    public void RemoveStaff(Guid id)
    {
        if (Staffs is null)
        {
            return;
        }

        var staff = Staffs?.FirstOrDefault(x => x.Id == id);

        if (staff is not null)
        { 
            Staffs!.Remove(staff);
        }
    }
}
