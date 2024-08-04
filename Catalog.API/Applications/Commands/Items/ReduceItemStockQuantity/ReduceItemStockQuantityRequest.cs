namespace Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;

public record ReduceItemStockQuantityRequest(string ItemId, int Quantity);
