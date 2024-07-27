namespace Catalog.Contracts;

public record ItemCreatedIntegrationEvent(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    DateTime CreatedAt,
    int StockQuantity,
    int SoldQuantity,
    string CategoryId,
    string CategoryName,
    string BrandId,
    string BrandName);