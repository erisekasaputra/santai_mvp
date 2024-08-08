using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.BusinessLicenseAggregate;

namespace Account.API.Mapper;

public static class BusinessLicenseMapperExtension
{
    public static IEnumerable<BusinessLicenseResponseDto>? ToBusinessLicenseResponseDtos(this ICollection<BusinessLicense>? businessLicenses)
    {
        if (businessLicenses is null)
        {
            yield break;    
        }

        foreach (var businessLicense in businessLicenses)
        {
            yield return businessLicense.ToBusinessLicenseResponseDto();
        }
    }

    public static BusinessLicenseResponseDto ToBusinessLicenseResponseDto(this BusinessLicense businessLicense) 
    {
        return new BusinessLicenseResponseDto(businessLicense.Id, businessLicense.LicenseNumber, businessLicense.Name, businessLicense.Description);
    }
}
