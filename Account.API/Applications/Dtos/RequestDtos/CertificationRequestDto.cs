using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
 
public class CertificationRequestDto(
    string certificationId,
    string certificationName,
    DateTime? validDate,
    IEnumerable<string>? specialization)
{
    public required string CertificationId { get; set; } = certificationId.Clean();
    public required string CertificationName { get; set; } = certificationName.Clean();
    public  DateTime? ValidDate { get; set; } = validDate;
    public required IEnumerable<string>? Specialization { get; set; } = specialization?.Select(x => x.Clean());
}
