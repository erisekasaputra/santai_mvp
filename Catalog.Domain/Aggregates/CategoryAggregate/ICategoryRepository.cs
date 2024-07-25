namespace Catalog.Domain.Aggregates.CategoryAggregate;

public interface ICategoryRepository
{ 
    Task<Category> CreateCategoryAsync(Category item);

    Task<Category?> GetCategoryByIdAsync(string id);

    Task<Category?> GetCategoryByNameAsync(string name);    

    Task<(int TotalCount, int TotalPages, IEnumerable<Category> Categories)> GetPaginatedCategoriesAsync(int pageNumber, int pageSize);

    void UpdateCategory(Category item);

    void DeleteCategory(Category item);

}
