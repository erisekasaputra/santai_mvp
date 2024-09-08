using Catalog.API.Applications.Dtos.Brand;
using Catalog.Domain.Aggregates.BrandAggregate;

namespace Catalog.API.Extensions;

public static class BrandMapperExtension
{
    public static IEnumerable<BrandDto> ToBrandsDto(this IEnumerable<Brand> brands)
    {
        foreach (var brand in brands)
        {
            yield return brand.ToBrandDto();
        }
    }

    public static BrandDto ToBrandDto(this Brand brand) 
    { 
        return new BrandDto(
                brand.Id,
                brand.Name,
                brand.ImageUrl
            );
    }
}
