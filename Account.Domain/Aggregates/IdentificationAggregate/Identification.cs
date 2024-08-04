using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.IdentificationAggregate;

public class Identification  : Entity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    public string IdentificationNumber { get; private set; }

    public IdentificationType Type { get; private set; }

    public string FrontSideImageUrl { get; private set; }

    public string BackSideImageUrl { get; private set; }

    public DocumentVerificationStatus IsVerified { get; private set; }

    private Identification(Guid userId, string identificationNumber, IdentificationType type, string frontSideImageUrl, string backSideImageUrl)
    {
        UserId = userId != default ? userId : throw new InvalidOperationException(nameof(userId));
        IdentificationNumber = identificationNumber ?? throw new ArgumentNullException(nameof(identificationNumber));
        FrontSideImageUrl = frontSideImageUrl ?? throw new ArgumentNullException(nameof(frontSideImageUrl));
        BackSideImageUrl = backSideImageUrl ?? throw new ArgumentNullException(nameof(backSideImageUrl));
        Type = type;
        IsVerified = DocumentVerificationStatus.Waiting;
    }

    public void VerifyDocument()
    {
        if (!IsVerified.Equals(DocumentVerificationStatus.Waiting))
        {
            throw new DomainException($"Can not change document verification status to {DocumentVerificationStatus.Accepted}");
        }

        IsVerified = DocumentVerificationStatus.Accepted;

        RaiseDocumentAcceptedDomainEvent();
    } 
   
    public void RejectDocument()
    {
        if (!IsVerified.Equals(DocumentVerificationStatus.Waiting))
        {
            throw new DomainException($"Can not change document verification status from {IsVerified} to {DocumentVerificationStatus.Accepted}");
        }

        IsVerified = DocumentVerificationStatus.Rejected;
         
        RaiseDocumentRejectedDomainEvent();
    } 

    public static Identification CreateNationalID(Guid userId, string identificationNumber, string frontSideImageUrl, string backSideImageUrl)
    {
        return new Identification(userId, identificationNumber, IdentificationType.NationalID, frontSideImageUrl, backSideImageUrl);
    }

    public static Identification CreateDrivingLicense(Guid userId, string identificationNumber, string frontSideImageUrl, string backSideImageUrl)
    {
        return new Identification(userId, identificationNumber, IdentificationType.DrivingLicense, frontSideImageUrl, backSideImageUrl);
    }

    private void RaiseDocumentAcceptedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDocumentRejectedDomainEvent()
    {
        throw new NotImplementedException();
    }
}
