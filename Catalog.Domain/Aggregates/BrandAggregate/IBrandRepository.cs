namespace Catalog.Domain.Aggregates.BrandAggregate;

public interface IBrandRepository
{
    Task<Brand> CreateBrandAsync(Brand item);

    Task<Brand?> GetBrandByIdAsync(string id);

    Task<Brand?> GetBrandByNameAsync(string name);

    Task<(int TotalCount, int TotalPages, IEnumerable<Brand> Brands)> GetPaginatedBrandsAsync(int pageNumber, int pageSize);

    void UpdateBrand(Brand item);  
}
