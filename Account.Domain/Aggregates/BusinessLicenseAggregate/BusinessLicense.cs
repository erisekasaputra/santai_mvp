using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Events;
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

    public VerificationState VerificationStatus { get; private set; } 

    public BusinessLicense()
    {

    }
   
    public BusinessLicense(Guid businessUserId, string licenseNumber, string name, string description)
    {
        BusinessUserId = businessUserId;
        Name = name;
        Description = description;
        LicenseNumber = licenseNumber;
        VerificationStatus = VerificationState.Waiting; 
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
        AddDomainEvent(new BusinessLicenseAcceptedDomainEvent());
    }

    private void RaiseDocumentRejectedDomainEvent()
    {
        AddDomainEvent(new BusinessLicenseRejectedDomainEvent());
    }

    private void RaiseVerificationStatusChangedException(VerificationState documentVerificationStatus)
    {
        throw new DomainException($"Can not change document verification status from {VerificationStatus} to {documentVerificationStatus}");
    }
}
