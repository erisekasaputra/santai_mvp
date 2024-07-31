using Catalog.API.DTOs.Category;
using Catalog.Domain.Aggregates.CategoryAggregate;

namespace Catalog.API.Extensions;

public static class CategoryExtension
{
    public static IEnumerable<CategoryDto> ToCategoriesDto(this IEnumerable<Category> categories)
    {
        foreach (var category in categories)
        {
            yield return category.ToCategoryDto();
        }
    }

    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto(
                category.Id,
                category.Name,
                category.ImageUrl
            );
    }
}
