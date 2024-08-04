namespace Catalog.API.Applications.Commands.Items.SetItemStockQuantity;

public record SetItemStockQuantityRequest(string ItemId, int Quantity);