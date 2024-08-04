using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.IdentificationAggregate;  
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using Account.Domain.Enumerations;

namespace Account.Domain.Aggregates.UserAggregate;

public class MechanicUser : User
{
    private List<Certification>? _certifications;

    public IReadOnlyCollection<Certification>? Certifications => _certifications?.AsReadOnly();

    public ICollection<DrivingLicense>? DrivingLicenses { get; private set; } // Navigation properties

    public ICollection<NationalIdentity>? NationalIdentities { get; private set; } // Navigation properties

    public decimal Rating { get; private set; }
     
    public bool IsVerified { get; private set; }

    public string DeviceId { get; private set; } 

    public MechanicUser() : base()
    {
        _certifications = [];
    } 

    public MechanicUser(
        Guid identityId,
        string username,
        string email,
        string phoneNumber,
        Address address,
        int referralRewardPoint,
        string deviceId) : base(identityId, username, email, phoneNumber, address, referralRewardPoint)
    {  
        Rating = 5;
        IsVerified = false;
        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseMechanicUserCreatedDomainEvent();
    }

    protected override void UpdateEmail(string email)
    {
        base.UpdateEmail(email);

        RaiseEmailUpdatedDomainEvent();
    }

    protected override void UpdatePhoneNumber(string phoneNumber)
    {
        base.UpdatePhoneNumber(phoneNumber);

        RaisePhoneNumberUpdatedDomainEvent();
    }

    protected override void VerifyEmail()
    {
        base.VerifyEmail();

        RaiseEmailVerifiedDomainEvent();
    }

    protected override void VerifyPhoneNumber()
    {
        base.VerifyPhoneNumber();

        RaisePhoneNumberVerifiedDomainEvent();
    }

    public void SetDrivingLicense(string identificationNumber, string frontSideImageUrl, string backSideImageUrl)
    {
        if (DrivingLicenses is not null)
        {
            throw new DomainException("Can not set Driving License once the Driving License is already set");
        } 

        if (IsVerified)
        {
            throw new DomainException($"Can not set Driving License once the document is verified");
        }

        DrivingLicenses ??= [];

        DrivingLicenses.Add(new DrivingLicense(Id, identificationNumber, frontSideImageUrl, backSideImageUrl));

        RaiseDrivingLicenseSetDomainEvent();
    }  

    public void SetNationalID(string identificationNumber, string frontSideImageUrl, string backSideImageUrl)
    {
        if (NationalIdentities is not null)
        {
            throw new DomainException("Can not set National ID once the National ID is already set");
        } 

        if (IsVerified)
        {
            throw new DomainException($"Can not set National ID once the document is verified");
        }

        NationalIdentities ??= [];

        NationalIdentities.Add(new NationalIdentity(Id, identificationNumber, frontSideImageUrl, backSideImageUrl));

        RaiseNationalIDSetDomainEvent();
    } 

    public void AddCertification(string certificationId, string certificationName, DateTime validDate, ICollection<string> specializations)
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

        _certifications ??= [];

        _certifications.Add(new Certification(Id, certificationId, certificationName, validDate, specializations));
    }

    public void RemoveCertification(Guid id)
    {
        if (_certifications is null)
        {
            return;
        }

        var certification = Certifications?.SingleOrDefault(e => e.Id == id);

        if (certification is not null)
        {  
            _certifications.Remove(certification);
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
        if ((DrivingLicenses is null || !DrivingLicenses.Any()) && (NationalIdentities is null || !NationalIdentities.Any()))
        {
            throw new DomainException("Both driving license and national identity are empty. Please submit valid documents.");
        }

        if (DrivingLicenses is null || !DrivingLicenses.Any())
        {
            throw new DomainException("Driving license document is empty. Please submit a valid document.");
        }

        if (NationalIdentities is null || !NationalIdentities.Any())
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
