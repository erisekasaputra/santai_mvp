using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.NationalIdentityAggregate;

public class NationalIdentity : Entity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    public MechanicUser MechanicUser { get; private set; }

    public string HashedIdentityNumber { get; private set; }

    public string EncryptedIdentityNumber { get; private set; }

    public string FrontSideImageUrl { get; private set; }

    public string BackSideImageUrl { get; private set; }

    public VerificationState VerificationStatus { get; private set; }
     
    public NationalIdentity(Guid userId, string hashedIdentityNumber, string encryptedIdentityNumber, string frontSideImageUrl, string backSideImageUrl) 
    {
        UserId = userId != default ? userId : throw new InvalidOperationException(nameof(userId));
        HashedIdentityNumber = hashedIdentityNumber ?? throw new ArgumentNullException(nameof(hashedIdentityNumber));
        EncryptedIdentityNumber = encryptedIdentityNumber ?? throw new ArgumentNullException(nameof(encryptedIdentityNumber));
        FrontSideImageUrl = frontSideImageUrl ?? throw new ArgumentNullException(nameof(frontSideImageUrl));
        BackSideImageUrl = backSideImageUrl ?? throw new ArgumentNullException(nameof(backSideImageUrl));
        VerificationStatus = VerificationState.Waiting; 
    }

    public void VerifyDocument()
    {
        if (!VerificationStatus.Equals(VerificationState.Waiting))
        {
            throw new DomainException($"Can not change verification status status to {VerificationState.Accepted}");
        }

        VerificationStatus = VerificationState.Accepted;

        RaiseDocumentAcceptedDomainEvent();
    }

    public void RejectDocument()
    {
        if (!VerificationStatus.Equals(VerificationState.Waiting))
        {
            throw new DomainException($"Can not change verification status from {VerificationStatus} to {VerificationState.Accepted}");
        }

        VerificationStatus = VerificationState.Rejected;

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
}
