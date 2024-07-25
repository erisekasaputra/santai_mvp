namespace Catalog.API.Commands.CreateItem;

public class CreateItemResponse(
    string id,
    string name,
    string description,
    decimal price,
    string imageUrl,
    DateTime createdAt,
    int stockQuantity,
    int soldQuantity,
    string itemCategoryId,
    string categoryName)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    public string ImageUrl { get; set; } = imageUrl;
    public DateTime CreatedAt { get; set; } = createdAt;
    public int StockQuantity { get; set; } = stockQuantity;
    public int SoldQuantity { get; set; } = soldQuantity;
    public string ItemCategoryId { get; set; } = itemCategoryId;
    public string CategoryName { get; set; } = categoryName;
    public static CreateItemResponse Create(
        string id,
        string name,
        string description,
        decimal price,
        string imageUrl,
        DateTime createdAt,
        int stockQuantity,
        int soldQuantity,
        string itemCategoryId,
        string categoryName)
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
            itemCategoryId,
            categoryName);
    }
}
