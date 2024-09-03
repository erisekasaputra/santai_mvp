namespace Catalog.API.Applications.Commands.Items.SetItemStockQuantity;

public record SetItemStockQuantityRequest(Guid ItemId, int Quantity);