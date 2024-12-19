using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.ValueObjects;
using Account.Domain.Enumerations;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Events;
using Core.Exceptions;
using Core.Extensions;

namespace Account.Domain.Aggregates.UserAggregate;

public class MechanicUser : BaseUser
{  
    public ICollection<decimal> Ratings { get; set; }
    public PersonalInfo PersonalInfo { get; private set; } 
    public ICollection<Certification>? Certifications { get; private set; } 
    public ICollection<DrivingLicense>? DrivingLicenses { get; private set; } 
    public ICollection<NationalIdentity>? NationalIdentities { get; private set; }  
    public VerificationState IsVerified { get; private set; } 
    public int TotalEntireJob { get; private set; } = 0;
    public int TotalCancelledJob { get; private set; } = 0;
    public int TotalEntireJobBothCompleteIncomplete { get; private set; } = 0;
    public int TotalCompletedJob { get; private set; } = 0;
    public bool IsActive { get; private set; } 
    public string DisablingReason { get; private set; }

    public void SetCompleteJob()
    {
        TotalEntireJobBothCompleteIncomplete += 1;
        TotalCompletedJob++;
    }

    public void SetIncompleteJob()
    { 
        TotalEntireJobBothCompleteIncomplete += 1;
    }

    public void AcceptJob()
    {
        TotalEntireJob += 1;
    }

    public void CancelByMechanic()
    {
        TotalCancelledJob += 1;
    }

    public void UnblockAccount()
    {
        if (IsVerified != VerificationState.Accepted) 
        {
            throw new DomainException("Your account has not been verified");
        }

        IsActive = true;
    }

    public void BlockAccount(string reason)
    {
        if (string.IsNullOrEmpty(reason))
        {
            throw new DomainException("Reason can not be empty");
        }

        IsActive = false;
        DisablingReason = reason;
    }

    public void UpdateProfilePicture(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        PersonalInfo.ProfilePictureUrl = path;
    }
    public decimal Rating
    {
        get
        {
            if (Ratings == null || Ratings.Count == 0)
            {
                return 5;
            }

            return Ratings.Average();
        }
    }

    protected MechanicUser() : base()
    { 
        PersonalInfo = null!;
        Ratings = [];
        DisablingReason = string.Empty;
    }

    public MechanicUser(
        Guid identityId, 
        string? email,
        string? encryptedEmail,
        string phoneNumber,
        string encryptedPhoneNumber,
        PersonalInfo personalInfo,
        Address address, 
        string timeZoneId ) : base(
            $"{personalInfo.FirstName} {personalInfo.MiddleName} {personalInfo.LastName}".CleanAndLowering(),
            email,
            encryptedEmail,
            phoneNumber,
            encryptedPhoneNumber,
            address,
            timeZoneId )
    {  
        Id = identityId;
        PersonalInfo = personalInfo;
        Ratings = [];
        IsVerified = VerificationState.Waiting;
        IsActive = false;
        DisablingReason = string.Empty;
        RaiseMechanicUserCreatedDomainEvent(this);
    } 

    public void Update(PersonalInfo personalInfo, Address address, string timeZoneId)
    {
        PersonalInfo = personalInfo;
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

    public void Delete()
    {  
        AddDomainEvent(new MechanicUserDeletedDomainEvent(Id));
    }

    public void SetDrivingLicense(
        string hashedLicenseNumber,
        string encryptedLicenseNumber,
        string frontSideImageUrl,
        string backSideImageUrl)
    {
        if (DrivingLicenses is not null && DrivingLicenses.Count > 0)
        {
            throw new DomainException("Can not set driving license because it is already set");
        } 

        if (IsVerified == VerificationState.Accepted)
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

        IsVerified = VerificationState.Waiting;

        RaiseDrivingLicenseSetDomainEvent();
    }  

    public void SetNationalID(
        string hashedIdentificationNumber,
        string encryptedIdentificationNumber,
        string frontSideImageUrl,
        string backSideImageUrl)
    {
        if (NationalIdentities is not null && NationalIdentities.Count > 0)
        {
            throw new DomainException("Can not set national id because it is already set");
        } 

        if (IsVerified == VerificationState.Accepted)
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

        IsVerified = VerificationState.Waiting;

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

    public void SetRating(decimal rating)
    {
        if (rating is < (decimal) 0.00 or > (decimal) 5.00)
        {
            throw new DomainException("Rating must be between 0.0 and 5.0");
        }

        Ratings ??= [];
        Ratings.Add(rating);
    } 

    public override string ToString()
    {
        return PersonalInfo is null ? string.Empty : $"{PersonalInfo.FirstName} {PersonalInfo.MiddleName} {PersonalInfo.LastName}";
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

        var verifiedDrivingLicense = DrivingLicenses.FirstOrDefault(x => x.VerificationStatus == VerificationState.Accepted || x.VerificationStatus == VerificationState.Waiting);
        var verifiedNationalIdentity = NationalIdentities.FirstOrDefault(x => x.VerificationStatus == VerificationState.Accepted || x.VerificationStatus == VerificationState.Waiting);

        if (verifiedDrivingLicense is null && verifiedNationalIdentity is null)
        {
            throw new DomainException("Both of national identity and driving license are rejected.");
        }

        if (verifiedDrivingLicense is null)
        {
            throw new DomainException("Driving license is rejected.");
        }

        if (verifiedNationalIdentity is null)
        {
            throw new DomainException("National identity is rejected.");
        }

        if (verifiedDrivingLicense.VerificationStatus == VerificationState.Waiting && verifiedNationalIdentity.VerificationStatus == VerificationState.Waiting)
        {
            throw new DomainException("Both of national identity and driving license are waiting for verification.");
        }

        if (verifiedDrivingLicense.VerificationStatus == VerificationState.Waiting)
        {
            throw new DomainException("Driving license is waiting for verification.");
        }

        if (verifiedNationalIdentity.VerificationStatus == VerificationState.Waiting)
        {
            throw new DomainException("National identity is waiting for verification.");
        }

        IsVerified = VerificationState.Accepted;  
        RaiseMechanicDocumentVerifiedDomainEvent(this);
    }


    public void RejectDocument()
    {
        if (IsVerified != VerificationState.Waiting) 
        {
            throw new DomainException("Verification must be in waiting stage");
        }
         
        IsVerified = VerificationState.Rejected;

        foreach (var identity in NationalIdentities ?? []) {
            identity.RejectDocument();
        }

        foreach (var license in DrivingLicenses ?? [])
        {
            license.RejectDocument();
        }

        RaiseMechanicDocumentRejectedDomainEvent(this);
    }

    private void RaiseMechanicDocumentVerifiedDomainEvent(MechanicUser user)
    {
        AddDomainEvent(new MechanicDocumentVerifiedDomainEvent(user));
    }

    private void RaiseMechanicDocumentRejectedDomainEvent(MechanicUser user)
    {
        AddDomainEvent(new MechanicDocumentRejectedDomainEvent(user));
    }

    private void RaiseMechanicUserCreatedDomainEvent(MechanicUser mechanisUser)
    {
        AddDomainEvent(new MechanicUserCreatedDomainEvent(mechanisUser));
    }

    private void RaiseDrivingLicenseSetDomainEvent()
    { 
    }

    private void RaiseNationalIDSetDomainEvent()
    { 
    } 
}
