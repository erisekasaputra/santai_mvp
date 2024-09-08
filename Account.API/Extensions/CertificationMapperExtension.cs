using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.CertificationAggregate;
using Core.Extensions;

namespace Account.API.Extensions;

public static class CertificationMapperExtension
{
    public static IEnumerable<CertificationResponseDto>? ToCertificationResponseDtos(this IEnumerable<Certification>? certifications, string timeZoneId)
    {
        if (certifications is null)
        {
            yield break;
        }

        foreach (var certification in certifications)
        {
            yield return certification.ToCertificationResponseDto(timeZoneId);
        }
    }

    public static CertificationResponseDto ToCertificationResponseDto(this Certification certifications, string timeZoneId)
    {
        return new CertificationResponseDto(
                certifications.CertificationId,
                certifications.CertificationName,
                certifications.ValidDateUtc.FromUtcToLocal(timeZoneId),
                certifications.Specializations
            );
    }
}
