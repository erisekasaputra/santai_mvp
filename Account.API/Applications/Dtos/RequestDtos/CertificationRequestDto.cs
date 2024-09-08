using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
 
public class CertificationRequestDto(
    string certificationId,
    string certificationName,
    DateTime? validDate,
    IEnumerable<string>? specialization)
{
    public string CertificationId { get; } = certificationId.Clean();
    public string CertificationName { get; } = certificationName.Clean();
    public DateTime? ValidDate { get; } = validDate;
    public IEnumerable<string>? Specialization { get; } = specialization?.Select(x => x.Clean());
}
