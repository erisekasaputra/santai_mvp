using Account.Domain.Aggregates.CertificationAggregate; 
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using Account.Domain.Enumerations;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Aggregates.DrivingLicenseAggregate;

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

        RaiseMechanicUserCreatedDomainEvent();
    }

    public override void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    {
        base.AddReferralProgram(referralRewardPoint, referralValidDate);
    }

    public override void UpdateEmail(string email, string encryptedEmail)
    {
        base.UpdateEmail(email, encryptedEmail);

        RaiseEmailUpdatedDomainEvent();
    }

    public override void UpdatePhoneNumber(string phoneNumber, string encryptedPhoneNumber)
    {
        base.UpdatePhoneNumber(phoneNumber, encryptedPhoneNumber);

        RaisePhoneNumberUpdatedDomainEvent();
    }

    public override void VerifyEmail()
    {
        base.VerifyEmail();

        RaiseEmailVerifiedDomainEvent();
    }

    public override void VerifyPhoneNumber()
    {
        base.VerifyPhoneNumber();

        RaisePhoneNumberVerifiedDomainEvent();
    }

    public void SetDrivingLicense(string identificationNumber, string encryptedIdentificationNumber, string frontSideImageUrl, string backSideImageUrl)
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

        DrivingLicenses.Add(new DrivingLicense(Id, identificationNumber, encryptedIdentificationNumber, frontSideImageUrl, backSideImageUrl));

        RaiseDrivingLicenseSetDomainEvent();
    }  

    public void SetNationalID(string identificationNumber, string encryptedIdentificationNumber, string frontSideImageUrl, string backSideImageUrl)
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

        NationalIdentities.Add(new NationalIdentity(Id, identificationNumber, encryptedIdentificationNumber, frontSideImageUrl, backSideImageUrl));

        RaiseNationalIDSetDomainEvent();
    } 

    public void AddCertification(string certificationId, string certificationName, DateTime validDate, ICollection<string>? specializations)
    { 
        var certifications = Certifications?.SingleOrDefault(c => c.CertificationId == certificationId);

        if (certifications is not null)
        {
            throw new DomainException($"Certification with certificate id {certificationId} is already registered");
        }

        if (validDate < DateTime.Now)
        {
            throw new DomainException($"You certificate with certificate id {certificationId} is expired");
        }

        Certifications ??= [];

        Certifications.Add(new Certification(Id, certificationId, certificationName, validDate, specializations));
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

        RaiseDeviceIdResetDomainEvent();
    }  

    public void SetDeviceId(string deviceId)
    {
        if (deviceId is not null)
        {
            throw new DomainException("This account is already registered with another device");
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdSetDomainEvent();
    }  

    public void ForceSetDeviceId(string deviceId)
    {
        if (DeviceId == deviceId)
        {
            return;
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdForcedSetDomainEvent();
    } 

    public void SetRating(decimal rating)
    {
        if (rating is < (decimal) 0.00 or > (decimal) 5.00)
        {
            throw new DomainException("Rating must be between 0.0 and 5.0.");
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

    private void RaiseEmailUpdatedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaisePhoneNumberUpdatedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseEmailVerifiedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaisePhoneNumberVerifiedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseMechanicUserCreatedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDrivingLicenseSetDomainEvent()
    {
        throw new NotImplementedException();
    }
    private void RaiseNationalIDSetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdResetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdSetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdForcedSetDomainEvent()
    {
        throw new NotImplementedException();
    }
}
