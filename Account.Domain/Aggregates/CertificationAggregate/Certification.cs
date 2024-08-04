using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.CertificationAggregate;

public class Certification : Entity, IAggregateRoot
{
    public Guid MechanicUserId { get; set; }

    public string CertificationId { get; private set; }

    public string CertificationName { get; private set; }

    public DateTime ValidDate { get; private set; }

    public ICollection<string>? Specializations { get; private set; }

    public Certification(Guid mechanicUserId, string certificationId, string certificationName, DateTime validDate, ICollection<string> specializations)
    {
        MechanicUserId = mechanicUserId != default ? mechanicUserId : throw new InvalidOperationException(nameof(mechanicUserId));
        CertificationId = certificationId ?? throw new ArgumentNullException(nameof(certificationId));
        CertificationName = certificationName ?? throw new ArgumentNullException(nameof(certificationName));
        ValidDate = validDate != default ? validDate : throw new InvalidOperationException(nameof(mechanicUserId));
        Specializations = specializations;
    }
}