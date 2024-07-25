namespace Catalog.API.Commands.Items.CreateItem;

public class CreateItemResponse(
    string id,
    string name,
    string description,
    decimal price,
    string imageUrl,
    DateTime createdAt,
    int stockQuantity,
    int soldQuantity,
    string categoryId,
    string categoryName,
    string brandId,
    string brandName)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    public string ImageUrl { get; set; } = imageUrl;
    public DateTime CreatedAt { get; set; } = createdAt;
    public int StockQuantity { get; set; } = stockQuantity;
    public int SoldQuantity { get; set; } = soldQuantity;
    public string ItemCategoryId { get; set; } = categoryId;
    public string CategoryName { get; set; } = categoryName;
    public string BrandId { get; set; } = brandId;
    public string BrandName { get; set; } = brandName;
    public static CreateItemResponse Create(
        string id,
        string name,
        string description,
        decimal price,
        string imageUrl,
        DateTime createdAt,
        int stockQuantity,
        int soldQuantity,
        string categoryId,
        string categoryName,
        string brandId,
        string brandName)
    {
        return new CreateItemResponse(
            id,
            name,
            description,
            price,
            imageUrl,
            createdAt,
            stockQuantity,
            soldQuantity,
            categoryId,
            categoryName,
            brandId,
            brandName);
    }
}
