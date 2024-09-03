namespace Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;

public record ReduceItemStockQuantityRequest(Guid ItemId, int Quantity);
