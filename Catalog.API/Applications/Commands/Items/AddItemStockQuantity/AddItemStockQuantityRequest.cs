namespace Catalog.API.Applications.Commands.Items.AddItemStockQuantity;

public record AddItemStockQuantityRequest(Guid ItemId, int Quantity);
