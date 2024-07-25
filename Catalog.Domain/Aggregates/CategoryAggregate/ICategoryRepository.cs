using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.CategoryAggregate;

public interface ICategoryRepository
{
    Task<Category?> GetCategoryByIdAsync(string id);
}
