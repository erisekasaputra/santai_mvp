using Account.Domain.Aggregates.CertificationAggregate; 
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using Account.Domain.Enumerations;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Events;

namespace Account.Domain.Aggregates.UserAggregate;

public class MechanicUser : User
{  
    public ICollection<Certification>? Certifications { get; private set; }

    public ICollection<DrivingLicense>? DrivingLicenses { get; private set; } // Navigation properties

    public ICollection<NationalIdentity>? NationalIdentities { get; private set; } // Navigation properties

    public decimal Rating { get; private set; }
     
    public bool IsVerified { get; private set; }

    public string? DeviceId { get; private set; }

    protected MechanicUser() : base()
    {

    }

    public MechanicUser(
        Guid identityId,
        string username,
        string email,
        string encryptedEmail,
        string phoneNumber,
        string encryptedPhoneNumber,
        Address address, 
        string timeZoneId,
        string deviceId) : base(identityId, username, email, encryptedEmail, phoneNumber, encryptedPhoneNumber, address, timeZoneId)
    {  
        Rating = 5;
        IsVerified = false;
        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseMechanicUserCreatedDomainEvent(this);
    }

    public void Update(Address address, string timeZoneId)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
    }

    public override void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    {
        base.AddReferralProgram(referralRewardPoint, referralValidDate);
    }

    public override void UpdateEmail(string hashedEmail, string encryptedEmail)
    {
        base.UpdateEmail(hashedEmail, encryptedEmail); 
    }

    public override void UpdatePhoneNumber(string hashedPhoneNumber, string encryptedPhoneNumber)
    {
        base.UpdatePhoneNumber(hashedPhoneNumber, encryptedPhoneNumber); 
    }

    public override void VerifyEmail()
    {
        base.VerifyEmail(); 
    }

    public override void VerifyPhoneNumber()
    {
        base.VerifyPhoneNumber(); 
    }

    public void SetDrivingLicense(
        string hashedLicenseNumber,
        string encryptedLicenseNumber,
        string frontSideImageUrl,
        string backSideImageUrl)
    {
        if (DrivingLicenses is not null)
        {
            throw new DomainException("Can not set driving license because it is already set");
        } 

        if (IsVerified)
        {
            throw new DomainException($"Can not set driving license because it is verified");
        }

        DrivingLicenses ??= [];

        DrivingLicenses.Add(new DrivingLicense(
            Id,
            hashedLicenseNumber,
            encryptedLicenseNumber,
            frontSideImageUrl,
            backSideImageUrl));

        RaiseDrivingLicenseSetDomainEvent();
    }  

    public void SetNationalID(
        string hashedIdentificationNumber,
        string encryptedIdentificationNumber,
        string frontSideImageUrl,
        string backSideImageUrl)
    {
        if (NationalIdentities is not null)
        {
            throw new DomainException("Can not set national id because it is already set");
        } 

        if (IsVerified)
        {
            throw new DomainException($"Can not set national id because it is verified");
        }

        NationalIdentities ??= [];

        NationalIdentities.Add(new NationalIdentity(
            Id,
            hashedIdentificationNumber,
            encryptedIdentificationNumber,
            frontSideImageUrl,
            backSideImageUrl));

        RaiseNationalIDSetDomainEvent();
    } 

    public void AddCertification(
        string certificationId,
        string certificationName,
        DateTime? validDate,
        ICollection<string>? specializations)
    { 
        var certifications = Certifications?.SingleOrDefault(c => c.CertificationId == certificationId);

        if (certifications is not null)
        {
            throw new DomainException($"Certificate '{certificationId}' is already registered");
        }

        if (validDate is not null && validDate < DateTime.UtcNow)
        {
            throw new DomainException($"Certificate '{certificationId}' is expired");
        }

        Certifications ??= [];

        Certifications.Add(new Certification(
            Id,
            certificationId,
            certificationName,
            validDate,
            specializations));
    }

    public void RemoveCertification(Guid id)
    {
        if (Certifications is null)
        {
            return;
        }

        var certification = Certifications?.SingleOrDefault(e => e.Id == id);

        if (certification is not null)
        {  
            Certifications?.Remove(certification);
        }
    }

    public void ResetDeviceId()
    {
        if (DeviceId is null)
        {
            return;
        }

        DeviceId = null;

        RaiseDeviceIdResetDomainEvent(Id);
    }  

    public void SetDeviceId(string deviceId)
    {
        if (deviceId is not null)
        {
            throw new DomainException("This account is already registered with another device");
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdSetDomainEvent(Id, deviceId);
    }  

    public void ForceSetDeviceId(string deviceId)
    {
        if (DeviceId == deviceId)
        {
            return;
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdForcedSetDomainEvent(Id, deviceId);
    } 

    public void SetRating(decimal rating)
    {
        if (rating is < (decimal) 0.00 or > (decimal) 5.00)
        {
            throw new DomainException("Rating must be between 0.0 and 5.0");
        }

        Rating = rating;
    }

    public void VerifyDocument()
    {
        if ((DrivingLicenses is null || DrivingLicenses.Count == 0) 
            && (NationalIdentities is null || NationalIdentities.Count == 0))
        {
            throw new DomainException("Both driving license and national identity are empty. Please submit valid documents.");
        }

        if (DrivingLicenses is null || DrivingLicenses.Count == 0)
        {
            throw new DomainException("Driving license document is empty. Please submit a valid document.");
        }

        if (NationalIdentities is null || NationalIdentities.Count == 0)
        {
            throw new DomainException("National identity document is empty. Please submit a valid document.");
        }

        var verifiedDrivingLicense = DrivingLicenses.FirstOrDefault(x => x.VerificationStatus == VerificationState.Accepted);
        var verifiedNationalIdentity = NationalIdentities.FirstOrDefault(x => x.VerificationStatus == VerificationState.Accepted);

        if (verifiedDrivingLicense is null && verifiedNationalIdentity is null)
        {
            throw new DomainException("Both driving license and national identity are rejected. Please re-submit documents.");
        }

        if (verifiedDrivingLicense is null)
        {
            throw new DomainException("Driving license document is rejected. Please re-submit document.");
        }

        if (verifiedNationalIdentity is null)
        {
            throw new DomainException("National identity document is rejected. Please re-submit document.");
        }

        IsVerified = true;
    }   

    private void RaiseMechanicUserCreatedDomainEvent(MechanicUser mechanisUser)
    {
        AddDomainEvent(new MechanicUserCreatedDomainEvent(this));
    }

    private void RaiseDrivingLicenseSetDomainEvent()
    { 
    }

    private void RaiseNationalIDSetDomainEvent()
    { 
    }

    private void RaiseDeviceIdResetDomainEvent(Guid id)
    {
        AddDomainEvent(new DeviceIdResetDomainEvent(id));
    }

    private void RaiseDeviceIdSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdSetDomainEvent(id, deviceId));
    }

    private void RaiseDeviceIdForcedSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdForcedSetDomainEvent(id, deviceId));
    }
}
