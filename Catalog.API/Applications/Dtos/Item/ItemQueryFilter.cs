namespace Catalog.API.Applications.Dtos.Item;

public class ItemQueryFilter
{
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public ItemQueryFilter(
        Guid? categoryId, 
        Guid? brandId)
    {
        CategoryId = categoryId;
        BrandId = brandId;  
    }
}
