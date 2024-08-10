using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.CertificationAggregate;

public class Certification : Entity, IAggregateRoot
{
    public Guid MechanicUserId { get; private init; }

    public MechanicUser MechanicUser { get; private set; }

    public string CertificationId { get; private set; }

    public string CertificationName { get; private set; }

    public DateTime? ValidDateUtc { get; private set; }

    public ICollection<string>? Specializations { get; private set; }
      
    public Certification(Guid mechanicUserId, string certificationId, string certificationName, DateTime? validDateUtc, ICollection<string>? specializations)
    {  
        if (ValidDateUtc != null && validDateUtc < DateTime.UtcNow)
        {
            throw new DomainException("Certificate date is expired");
        }

        MechanicUserId = mechanicUserId != default ? mechanicUserId : throw new InvalidOperationException(nameof(mechanicUserId));
        CertificationId = certificationId ?? throw new ArgumentNullException(nameof(certificationId));
        CertificationName = certificationName ?? throw new ArgumentNullException(nameof(certificationName));
        ValidDateUtc = validDateUtc;
        Specializations = specializations; 
    }

    public void Update(string certificationId, string certificationName, DateTime validDateUtc, ICollection<string> specializations)
    {
        CertificationId = certificationId;
        CertificationName = certificationName;
        ValidDateUtc = validDateUtc;
        Specializations = specializations;
    }
}