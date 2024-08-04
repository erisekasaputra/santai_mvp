using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.BusinessLicenseAggregate;

public class BusinessLicense : Entity, IAggregateRoot
{
    public Guid BusinessUserId { get; private set; }

    public BusinessUser BusinessUser { get; private set; }

    public string LicenseNumber { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; } 

    public DocumentVerificationStatus VerificationStatus { get; private set; }

    public BusinessLicense()
    {

    }

    public BusinessLicense(Guid businessUserId, string licenseNumber, string name, string description)
    {
        BusinessUserId = businessUserId;
        Name = name;
        Description = description;
        LicenseNumber = licenseNumber;
        VerificationStatus = DocumentVerificationStatus.Waiting;
        BusinessUser = new();
    }
    public void VerifyDocument()
    {
        if (!VerificationStatus.Equals(DocumentVerificationStatus.Waiting))
        {
            RaiseVerificationStatusChangedException(DocumentVerificationStatus.Accepted);
        }

        VerificationStatus = DocumentVerificationStatus.Accepted;

        RaiseDocumentAcceptedDomainEvent();
    }

    public void RejectDocument()
    {
        if (!VerificationStatus.Equals(DocumentVerificationStatus.Waiting))
        {
            RaiseVerificationStatusChangedException(DocumentVerificationStatus.Rejected);
        }

        VerificationStatus = DocumentVerificationStatus.Rejected;

        RaiseDocumentRejectedDomainEvent();
    }

    private void RaiseDocumentAcceptedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDocumentRejectedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseVerificationStatusChangedException(DocumentVerificationStatus documentVerificationStatus)
    {
        throw new DomainException($"Can not change document verification status from {VerificationStatus} to {documentVerificationStatus}");
    }
}
