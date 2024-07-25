namespace Catalog.API.DTOs.ItemDto;

public class ItemDto(string id,
                  string name,
                  string description,
                  decimal price,
                  string imageUrl,
                  DateTime createdAt,
                  int stockQuantity,
                  int soldQuantity,
                  string categoryId,
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
    public string CategoryId { get; set; } = categoryId;
    public string CategoryName { get; set; } = categoryName;

    public static ItemDto Create(string id, string name, string description, decimal price, string imageUrl, DateTime createdAt, int stockQuantity, int soldQuantity, string categoryId, string categoryName)
    {
        return new ItemDto(id, name, description, price, imageUrl, createdAt, stockQuantity, soldQuantity, categoryId, categoryName);
    }
}
