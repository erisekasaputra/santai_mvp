using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

/// <summary>
/// Certification request Dto 
/// </summary>
/// <param name="CertificationId"></param>
/// <param name="CertificationName"></param>
/// <param name="ValidDate">Must be converted into utc time format</param>
/// <param name="Specialization"></param>
public class CertificationRequestDto(
    string CertificationId,
    string CertificationName,
    DateTime ValidDate,
    IEnumerable<string>? Specialization)
{
    public string CertificationId { get; } = CertificationId.Clean();
    public string CertificationName { get; } = CertificationName.Clean();
    public DateTime ValidDate { get; } = ValidDate;
    public IEnumerable<string>? Specialization { get; } = Specialization?.Select(x => x.Clean());
}
