using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.IdentificationAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public class MechanicUser : User
{
    private List<Certification>? _certifications;

    public IReadOnlyCollection<Certification>? Certifications => _certifications?.AsReadOnly();

    public Identification? DrivingLicense { get; private set; }

    public Identification? NationalID { get; private set; }

    public decimal Rating { get; private set; }
     
    public bool VerificationStatus { get; private set; }

    public string? DeviceId { get; private set; } 

    public MechanicUser() : base()
    {
        _certifications = [];
    } 

    public MechanicUser(
        string username,
        string email,
        string phoneNumber,
        Address address,
        string deviceId) : base(username, email, phoneNumber, address)
    {  
        Rating = 5;
        VerificationStatus = false;
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
        if (DrivingLicense is not null)
        {
            throw new DomainException("Can not set Driving License once the Driving License is already set");
        } 

        if (VerificationStatus)
        {
            throw new DomainException($"Can not set Driving License once the document is verified");
        }

        DrivingLicense = Identification.CreateDrivingLicense(Id, identificationNumber, frontSideImageUrl, backSideImageUrl);

        RaiseDrivingLicenseSetDomainEvent();
    }  

    public void SetNationalID(string identificationNumber, string frontSideImageUrl, string backSideImageUrl)
    {
        if (NationalID is not null)
        {
            throw new DomainException("Can not set National ID once the National ID is already set");
        } 

        if (VerificationStatus)
        {
            throw new DomainException($"Can not set National ID once the document is verified");
        }

        NationalID = Identification.CreateNationalID(Id, identificationNumber, frontSideImageUrl, backSideImageUrl);

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
        ValidateDocumentPresence();
        
        ValidateDocumentVerification();

        VerificationStatus = true;
    } 

    private void ValidateDocumentPresence()
    {
        if (DrivingLicense is null && NationalID is null)
        {
            throw new DomainException("Both driving license and national ID are empty. Please submit valid documents.");
        }

        if (DrivingLicense is null)
        {
            throw new DomainException("Driving license document is empty. Please submit a valid document.");
        }

        if (NationalID is null)
        {
            throw new DomainException("National ID document is empty. Please submit a valid document.");
        } 
    }

    private void ValidateDocumentVerification()
    {
        if (DrivingLicense is not null && DrivingLicense.IsVerified == DocumentVerificationStatus.Accepted)
        {
            throw new DomainException("The driving license must be verified first.");
        }

        if (NationalID is not null && NationalID.IsVerified == DocumentVerificationStatus.Accepted)
        {
            throw new DomainException("The national ID must be verified first.");
        }
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
