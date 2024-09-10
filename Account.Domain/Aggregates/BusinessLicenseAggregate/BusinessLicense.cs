using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Events; 
using Account.Domain.SeedWork;
using Core.Exceptions;

namespace Account.Domain.Aggregates.BusinessLicenseAggregate;

public class BusinessLicense : Entity, IAggregateRoot
{
    public Guid BusinessUserId { get; private set; }

    public BusinessUser BusinessUser { get; private set; }

    public string HashedLicenseNumber { get; private set; }

    public string EncryptedLicenseNumber { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; } 

    public VerificationState VerificationStatus { get; private set; } 

    public BusinessLicense()
    {
        BusinessUser = null!;
        HashedLicenseNumber = null!;
        EncryptedLicenseNumber = null!;
        Name = null!;
        Description = null!;
    }
   
    public BusinessLicense(
        Guid businessUserId,
        string hashedLicenseNumber,
        string encryptedLicenseNumber,
        string name,
        string description)
    {
        BusinessUserId = businessUserId;
        Name = name;
        Description = description;
        HashedLicenseNumber = hashedLicenseNumber;
        EncryptedLicenseNumber = encryptedLicenseNumber;
        VerificationStatus = VerificationState.Waiting;
        BusinessUser = null!;
    }

    public void VerifyDocument()
    {
        if (!VerificationStatus.Equals(VerificationState.Waiting))
        {
            RaiseVerificationStatusChangedException(VerificationState.Accepted);
        }

        VerificationStatus = VerificationState.Accepted;

        RaiseDocumentAcceptedDomainEvent();
    }

    public void RejectDocument()
    {
        if (!VerificationStatus.Equals(VerificationState.Waiting))
        {
            RaiseVerificationStatusChangedException(VerificationState.Rejected);
        }

        VerificationStatus = VerificationState.Rejected;

        RaiseDocumentRejectedDomainEvent();
    }

    private void RaiseDocumentAcceptedDomainEvent()
    {
        AddDomainEvent(new BusinessLicenseAcceptedDomainEvent(Id));
    }

    private void RaiseDocumentRejectedDomainEvent()
    {
        AddDomainEvent(new BusinessLicenseRejectedDomainEvent(Id));
    }

    private void RaiseVerificationStatusChangedException(VerificationState documentVerificationStatus)
    {
        throw new DomainException($"Can not change document verification status from {VerificationStatus} to {documentVerificationStatus}");
    }
}
