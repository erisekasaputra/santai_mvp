namespace Account.API.Applications.Dtos.ResponseDtos;

/// <summary>
/// Certification response Dto 
/// </summary>
/// <param name="CertificationId"></param>
/// <param name="CertificationName"></param>
/// <param name="ValidDate">Must be converted into utc time format</param>
/// <param name="Specialization"></param>
public record CertificationResponseDto(
    string CertificationId,
    string CertificationName,
    DateTime? ValidDate,
    IEnumerable<string>? Specialization);